using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorApp
{
    public class AppDbContextLogger : SaveChangesInterceptor
    {
        private readonly IServiceProvider _serviceProvider;
        private List<(EntityEntry Entry, EntityState State)> _addedEntries;

        public AppDbContextLogger(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _addedEntries = new List<(EntityEntry, EntityState)>();
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, 
            InterceptionResult<int> result)
        {
            PrepareSaveChanges(eventData).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, 
            InterceptionResult<int> result, 
            CancellationToken cancellationToken = default)
        {
            await PrepareSaveChanges(eventData);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            LogAddedEntities(eventData).GetAwaiter().GetResult();
            return base.SavedChanges(eventData, result);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, 
            int result, 
            CancellationToken cancellationToken = default)
        {
            await LogAddedEntities(eventData);
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private async Task PrepareSaveChanges(DbContextEventData eventData)
        {
            if (eventData.Context == null) return;

            try
            {
                _addedEntries.Clear();
                
                var entries = eventData.Context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || 
                                e.State == EntityState.Modified || 
                                e.State == EntityState.Deleted)
                    .ToList();

                foreach (var entry in entries.Where(e => e.State == EntityState.Added))
                {
                    _addedEntries.Add((entry, EntityState.Added));
                }

                var modifiedOrDeletedEntries = entries
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted)
                    .ToList();

                foreach (var entry in modifiedOrDeletedEntries)
                {
                    await LogEntry(entry, entry.State);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PrepareSaveChanges: {ex.Message}");
            }
        }

        private async Task LogAddedEntities(SaveChangesCompletedEventData eventData)
        {
            if (eventData.Context == null || _addedEntries.Count == 0) return;

            try
            {
                foreach (var (entry, state) in _addedEntries)
                {
                    await LogEntry(entry, state);
                }
                
                _addedEntries.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LogAddedEntities: {ex.Message}");
            }
        }

        private async Task LogEntry(EntityEntry entry, EntityState state)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var logService = scope.ServiceProvider.GetRequiredService<ActivityLogService>();
                
                string action = GetActionName(state);
                string entityType = entry.Entity.GetType().Name;
                string entityId = TryGetEntityId(entry);
                
                string details = BuildChangeDetails(entry, action);
                
                await logService.LogActivity(action, entityType, entityId, details);
                
                Console.WriteLine($"Logged: {action} {entityType} {entityId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging entity: {ex.Message}");
            }
        }

        private string GetActionName(EntityState state)
        {
            return state switch
            {
                EntityState.Added => "Create",
                EntityState.Modified => "Update",
                EntityState.Deleted => "Delete",
                _ => "Unknown"
            };
        }

        private string TryGetEntityId(EntityEntry entry)
        {
            try
            {
                foreach (var propertyName in new[] { "Id", "ID", entry.Entity.GetType().Name + "Id" })
                {
                    var property = entry.Properties.FirstOrDefault(p => 
                        string.Equals(p.Metadata.Name, propertyName, StringComparison.OrdinalIgnoreCase));
                    
                    if (property != null && property.CurrentValue != null)
                    {
                        return property.CurrentValue.ToString();
                    }
                }

                var keyProperties = entry.Properties
                    .Where(p => p.Metadata.IsPrimaryKey())
                    .ToList();
                
                if (keyProperties.Any())
                {
                    return string.Join(",", keyProperties.Select(p => p.CurrentValue?.ToString() ?? "null"));
                }
            }
            catch
            {
                // Ігноруємо помилки при пошуку ID
            }
            
            return "Unknown";
        }

        private string BuildChangeDetails(EntityEntry entry, string action)
        {
            if (action == "Update")
            {
                var changedProps = entry.Properties
                    .Where(p => p.IsModified && !p.Metadata.IsPrimaryKey())
                    .ToList();
                
                if (changedProps.Any())
                {
                    var changes = changedProps
                        .Select(p => $"{p.Metadata.Name}: {p.OriginalValue} -> {p.CurrentValue}")
                        .ToList();
                    
                    return $"Modified fields: {string.Join(", ", changes)}";
                }
            }
            
            string entityType = entry.Entity.GetType().Name;
            string entityId = TryGetEntityId(entry);
            return $"Entity {entityType} with ID {entityId} was {action.ToLower()}d.";
        }
    }
}
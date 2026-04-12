namespace ImbuziSmart.Shared.Sync;

// ── Outgoing (client → server) ────────────────────────────────────────────────

/// <summary>One change entry queued while offline.</summary>
public class SyncEntry
{
    /// <summary>The local IndexedDB syncQueue row ID (for acknowledgement).</summary>
    public string QueueId { get; set; } = "";

    /// <summary>IndexedDB store name, e.g. "animals", "weightRecords".</summary>
    public string EntityType { get; set; } = "";

    public Guid EntityId { get; set; }

    /// <summary>"upsert" or "delete"</summary>
    public string Action { get; set; } = "upsert";

    /// <summary>Full JSON of the entity — populated for upserts; null for deletes.</summary>
    public string? Payload { get; set; }
}

public class SyncPushRequest
{
    public List<SyncEntry> Changes { get; set; } = new();
}

// ── Incoming (server → client) ────────────────────────────────────────────────

public class SyncPushResult
{
    public bool Success { get; set; }
    public int Applied { get; set; }
    public List<SyncEntryError> Errors { get; set; } = new();
}

public class SyncEntryError
{
    public string QueueId { get; set; } = "";
    public string EntityType { get; set; } = "";
    public Guid EntityId { get; set; }
    public string Error { get; set; } = "";
}

// ── Local queue entry (client-only, mirrors IndexedDB row) ───────────────────

public class SyncQueueEntry
{
    public string Id { get; set; } = "";
    public string EntityType { get; set; } = "";
    public string EntityId { get; set; } = "";

    /// <summary>"upsert" or "delete"</summary>
    public string Action { get; set; } = "";

    public string Timestamp { get; set; } = "";
}

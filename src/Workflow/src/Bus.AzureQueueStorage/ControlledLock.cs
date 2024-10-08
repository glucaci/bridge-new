﻿using Microsoft.WindowsAzure.Storage.Blob;

namespace Bridge.Workflow.Bus.AzureQueueStorage;

class ControlledLock
{
    public string Id { get; set; }
    public string LeaseId { get; set; }
    public CloudBlockBlob Blob { get; set; }

    public ControlledLock(string id, string leaseId, CloudBlockBlob blob)
    {
        Id = id;
        LeaseId = leaseId;
        Blob = blob;
    }
}
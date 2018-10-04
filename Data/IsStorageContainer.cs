using System;
using Starship.Core.Data.Storage;

namespace Starship.Core.Data
{
    public interface IsStorageContainer
    {
        UploadedFile Upload(string blobName, byte[] data);
    }
}

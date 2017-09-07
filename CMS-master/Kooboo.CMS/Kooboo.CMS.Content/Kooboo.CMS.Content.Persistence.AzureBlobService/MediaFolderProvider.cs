﻿#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Kooboo.CMS.Content.Models.Paths;
using Kooboo.CMS.Content.Models;
using System.Runtime.Serialization;
using Kooboo.Runtime.Serialization;
using Ionic.Zip;
using Kooboo.CMS.Content.Services;
using System.IO;
using Kooboo.CMS.Common.Persistence.Non_Relational;
using Kooboo.Web.Url;
using Kooboo.CMS.Content.Caching;
using Kooboo.CMS.Caching;
namespace Kooboo.CMS.Content.Persistence.AzureBlobService
{

    public class MediaFolders
    {
        static System.Threading.ReaderWriterLockSlim locker = new System.Threading.ReaderWriterLockSlim();
        public static void AddFolder(MediaFolder folder)
        {
            locker.EnterWriteLock();
            try
            {
                var folders = GetList(folder.Repository);
                var name = folder.FullName.TrimStart('~').TrimEnd('~');
                if (!folders.ContainsKey(name))
                {
                    folders[name] = folder;
                    SaveList(folder.Repository, folders);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
        public static MediaFolder GetFolder(MediaFolder dummy)
        {
            locker.EnterReadLock();
            try
            {
                var folders = GetList(dummy.Repository);
                if (folders.ContainsKey(dummy.FullName))
                {
                    return ToMediaFolder(dummy.Repository, dummy.FullName, folders[dummy.FullName]);
                }
                return null;
            }
            finally
            {
                locker.ExitReadLock();
            }

        }
        public static void RemoveFolder(MediaFolder folder)
        {
            locker.EnterWriteLock();
            try
            {
                var storeList = GetList(folder.Repository);
                var mediaFolders = ToMediaFolders(folder.Repository, storeList);
                if (storeList.ContainsKey(folder.FullName))
                {
                    storeList.Remove(folder.FullName);

                    foreach (var item in mediaFolders)
                    {
                        if (item.Parent == folder)
                        {
                            if (storeList.ContainsKey(item.FullName))
                            {
                                storeList.Remove(item.FullName);
                            }
                        }
                    }
                }
                SaveList(folder.Repository, storeList);
            }
            finally
            {
                locker.ExitWriteLock();
            }

        }
        public static void UpdateFolder(MediaFolder folder)
        {
            locker.EnterWriteLock();
            try
            {
                var folders = GetList(folder.Repository);
                folders[folder.FullName] = folder;
                SaveList(folder.Repository, folders);
            }
            finally
            {
                locker.ExitWriteLock();
            }

        }
        public static void RenameFolder(MediaFolder @new, MediaFolder old)
        {

            locker.EnterWriteLock();
            try
            {
                var folders = GetList(old.Repository);
                var keys = folders.Keys.ToList();
                foreach (var key in keys)
                {
                    if (key.StartsWith(old.FullName + "~"))
                    {
                        var newKey = @new.FullName + key.Substring(old.FullName.Length);
                        folders.Add(newKey, folders[key]);
                        folders.Remove(key);
                    }
                }
                if (folders.ContainsKey(old.FullName) && !folders.ContainsKey(@new.FullName))
                {
                    folders.Add(@new.FullName, @new);
                    folders.Remove(@old.FullName);
                    //folders[old.FullName] = folder;
                    SaveList(@new.Repository, folders);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public static IEnumerable<MediaFolder> RootFolders(Repository repository)
        {
            locker.EnterReadLock();
            try
            {
                return ToMediaFolders(repository, GetList(repository)).Where(it => it.Parent == null);
            }
            finally
            {
                locker.ExitReadLock();
            }

        }
        public static IEnumerable<MediaFolder> ChildFolders(MediaFolder parent)
        {
            locker.EnterReadLock();
            try
            {
                var query = ToMediaFolders(parent.Repository, GetList(parent.Repository));
                //loop bug in azure
                query = query.Where(it => (parent == null && it.Parent == null) || (it.Parent != null && it.Parent.UUID == parent.UUID));
                return query;
            }
            finally
            {
                locker.ExitReadLock();
            }

        }
        private static IEnumerable<MediaFolder> ToMediaFolders(Repository repository, Dictionary<string, MediaFolder> folders)
        {
            return folders.Select(it => ToMediaFolder(repository, it.Key, it.Value)).ToArray();
        }
        private static MediaFolder ToMediaFolder(Repository repository, string fullName, MediaFolder folderProperties)
        {
            return new MediaFolder(repository, fullName)
            {
                DisplayName = folderProperties.DisplayName,
                UserId = folderProperties.UserId,
                UtcCreationDate = folderProperties.UtcCreationDate,
                AllowedExtensions = folderProperties.AllowedExtensions
            };
        }
        public static Dictionary<string, MediaFolder> GetList(Repository repository)
        {

            var mediaFolders = repository.ObjectCache().GetCache<Dictionary<string, MediaFolder>>("Azure-MediaFolders-Cachings", () =>
            {
                var container = MediaBlobHelper.InitializeRepositoryContainer(repository);

                var blobClient = CloudStorageAccountHelper.GetStorageAccount().CreateCloudBlobClient();

                var folderBlob = container.GetBlobReference(MediaBlobHelper.MediaDirectoryName);

                Dictionary<string, MediaFolder> folders = null;
                try
                {
                    folderBlob.FetchAttributes();
                    if (folderBlob.CheckIfMediaFolder())
                    {
                        var xml = folderBlob.DownloadText();
                        folders = DataContractSerializationHelper.DeserializeFromXml<Dictionary<string, MediaFolder>>(xml);
                    }

                }
                catch { }
                if (folders == null)
                {
                    folders = new Dictionary<string, MediaFolder>();
                }
                return new Dictionary<string, MediaFolder>(folders, StringComparer.OrdinalIgnoreCase);
            });
            return mediaFolders;
        }
        private static void SaveList(Repository repository, Dictionary<string, MediaFolder> folders)
        {
            if (folders != null && folders.Count > 0)
            {
                var xml = DataContractSerializationHelper.SerializeAsXml(folders);

                var container = MediaBlobHelper.InitializeRepositoryContainer(repository);

                var blobClient = CloudStorageAccountHelper.GetStorageAccount().CreateCloudBlobClient();

                var folderBlob = container.GetBlobReference(MediaBlobHelper.MediaDirectoryName);

                folderBlob.SetMediaFolderContentType();

                BlobRequestOptions header = new BlobRequestOptions();

                folderBlob.UploadText(xml);
            }
            else
            {
                var container = MediaBlobHelper.InitializeRepositoryContainer(repository);

                var blobClient = CloudStorageAccountHelper.GetStorageAccount().CreateCloudBlobClient();

                var folderBlob = container.GetBlobReference(MediaBlobHelper.MediaDirectoryName);

                folderBlob.DeleteIfExists();
            }
        }

    }
    [Kooboo.CMS.Common.Runtime.Dependency.Dependency(typeof(IMediaFolderProvider), Order = 2)]
    [Kooboo.CMS.Common.Runtime.Dependency.Dependency(typeof(IProvider<MediaFolder>), Order = 2)]
    public class MediaFolderProvider : IMediaFolderProvider
    {
        public IQueryable<MediaFolder> ChildFolders(MediaFolder parent)
        {
            return MediaFolders.ChildFolders(parent).AsQueryable();
        }

        public IEnumerable<MediaFolder> All(Repository repository)
        {
            return MediaFolders.RootFolders(repository).AsQueryable();
        }

        public MediaFolder Get(MediaFolder dummy)
        {
            return MediaFolders.GetFolder(dummy);
        }

        public void Add(MediaFolder item)
        {
            MediaFolders.AddFolder(item);
        }

        public void Update(MediaFolder @new, MediaFolder old)
        {
            MediaFolders.UpdateFolder(@new);
        }

        public void Remove(MediaFolder item)
        {
            MediaFolders.RemoveFolder(item);
            (new MediaContentProvider()).Delete(item);
        }


        public void Export(Repository repository, IEnumerable<MediaFolder> models, System.IO.Stream outputStream)
        {
            throw new NotImplementedException();
        }

        public void Import(Repository repository, MediaFolder folder, System.IO.Stream zipStream, bool @override)
        {
            using (ZipFile zipFile = ZipFile.Read(zipStream))
            {
                foreach (ZipEntry item in zipFile)
                {
                    if (item.IsDirectory)
                    {

                    }
                    else
                    {
                        var path = Path.GetDirectoryName(item.FileName);
                        if (!string.IsNullOrEmpty(path))
                        {
                            path = string.Join("~", path.Split('\\').ToArray());
                        }
                        var fileName = Path.GetFileName(item.FileName);
                        if (fileName.ToLower() != "setting.config")
                        {
                            var currentFolder = CreateMediaFolderByPath(folder, path);
                            Add(currentFolder);
                            var stream = new MemoryStream();
                            item.Extract(stream);
                            stream.Position = 0;
                            ServiceFactory.MediaContentManager.Add(repository, currentFolder,
                                fileName, stream, true);
                        }
                    }
                }
            }
        }
        private MediaFolder CreateMediaFolderByPath(MediaFolder folder, string pathName)
        {
            return new MediaFolder(folder.Repository, pathName, folder);
        }

        public IEnumerable<MediaFolder> All()
        {
            throw new NotImplementedException();
        }

        public void Rename(MediaFolder @new, MediaFolder old)
        {
            MediaFolders.RenameFolder(@new, old);

            var blobClient = CloudStorageAccountHelper.GetStorageAccount().CreateCloudBlobClient();
            //var dir = blobContainer.GetDirectoryReference();
            var oldPrefix = old.GetMediaFolderItemPath(null) + "/";
            var newPrefix = @new.GetMediaFolderItemPath(null) + "/";
            MoveDirectory(blobClient, newPrefix, oldPrefix);
        }


        public void Export(Repository repository, string baseFolder, string[] folders, string[] docs, Stream outputStream)
        {
            ZipFile zipFile = new ZipFile();
            var basePrefix = StorageNamesEncoder.EncodeContainerName(repository.Name) + "/" + MediaBlobHelper.MediaDirectoryName + "/";
            if (!string.IsNullOrEmpty(baseFolder))
            {
                var baseMediaFolder = ServiceFactory.MediaFolderManager.Get(repository, baseFolder);
                basePrefix = baseMediaFolder.GetMediaFolderItemPath(null) + "/";
            }

            var blobClient = CloudStorageAccountHelper.GetStorageAccount().CreateCloudBlobClient();

            //add file
            if (docs != null)
            {
                foreach (var doc in docs)
                {
                    var blob = blobClient.GetBlockBlobReference(basePrefix + StorageNamesEncoder.EncodeBlobName(doc));

                    var bytes = blob.DownloadByteArray();
                    zipFile.AddEntry(doc, bytes);
                }
            }
            //add folders
            if (folders != null)
            {
                foreach (var folder in folders)
                {
                    var folderName = folder.Split('~').LastOrDefault();
                    zipFolder(blobClient, basePrefix, folderName, "", ref zipFile);
                }
            }
            zipFile.Save(outputStream);

        }

        private void zipFolder(CloudBlobClient blobClient, string basePrefix, string folderName, string zipDir, ref ZipFile zipFile)
        {
            zipDir = string.IsNullOrEmpty(zipDir) ? folderName : zipDir + "/" + folderName;
            zipFile.AddDirectoryByName(zipDir);
            var folderPrefix = basePrefix + StorageNamesEncoder.EncodeBlobName(folderName) + "/";

            var blobs = blobClient.ListBlobsWithPrefix(folderPrefix,
new BlobRequestOptions() { BlobListingDetails = Microsoft.WindowsAzure.StorageClient.BlobListingDetails.Metadata, UseFlatBlobListing = false });
            foreach (var blob in blobs)
            {
                if (blob is CloudBlobDirectory)
                {
                    var dir = blob as CloudBlobDirectory;

                    var names = dir.Uri.ToString().Split('/');
                    for (var i = names.Length - 1; i >= 0; i--)
                    {
                        if (!string.IsNullOrEmpty(names[i]))
                        {
                            zipFolder(blobClient, folderPrefix, StorageNamesEncoder.DecodeBlobName(names[i]), zipDir, ref zipFile);
                            break;
                        }
                    }
                }
                if (blob is CloudBlob)
                {
                    var cloudBlob = blob as CloudBlob;
                    var subStr = cloudBlob.Uri.ToString().Substring(cloudBlob.Uri.ToString().IndexOf(folderPrefix) + folderPrefix.Length);
                    var index = subStr.LastIndexOf('/');
                    if (index < 0)
                    {
                        index = 0;
                    }

                    var subFolderName = subStr.Substring(0, index);
                    var fileName = subStr.Substring(index);
                    var bytes = cloudBlob.DownloadByteArray();
                    if (!string.IsNullOrEmpty(subFolderName))
                    {
                        zipFile.AddDirectoryByName(zipDir + "/" + StorageNamesEncoder.DecodeBlobName(subFolderName));
                    }
                    zipFile.AddEntry(zipDir + "/" + StorageNamesEncoder.DecodeBlobName(subStr), bytes);
                }
            }
        }

        private void MoveDirectory(CloudBlobClient blobClient, string newPrefix, string oldPrefix)
        {
            var blobs = blobClient.ListBlobsWithPrefix(oldPrefix,
                new BlobRequestOptions() { BlobListingDetails = Microsoft.WindowsAzure.StorageClient.BlobListingDetails.Metadata, UseFlatBlobListing = false });
            foreach (var blob in blobs)
            {
                if (blob is CloudBlobDirectory)
                {
                    var dir = blob as CloudBlobDirectory;

                    var names = dir.Uri.ToString().Split('/');
                    for (var i = names.Length - 1; i >= 0; i--)
                    {
                        if (!string.IsNullOrEmpty(names[i]))
                        {
                            MoveDirectory(blobClient, newPrefix + StorageNamesEncoder.EncodeBlobName(names[i]) + "/", oldPrefix + StorageNamesEncoder.EncodeBlobName(names[i]) + "/");
                            break;
                        }
                    }
                }
                else if (blob is CloudBlob)
                {
                    var cloudBlob = blob as CloudBlob;

                    if (cloudBlob.Exists())
                    {
                        cloudBlob.FetchAttributes();
                        var newContentBlob = blobClient.GetBlockBlobReference(newPrefix + cloudBlob.Metadata["FileName"]);
                        try
                        {
                            newContentBlob.CopyFromBlob(cloudBlob);
                        }
                        catch (Exception e)
                        {
                            using (Stream stream = new MemoryStream())
                            {
                                cloudBlob.DownloadToStream(stream);
                                stream.Position = 0;
                                newContentBlob.UploadFromStream(stream);
                                stream.Dispose();
                            }
                        }
                        newContentBlob.Metadata["FileName"] = cloudBlob.Metadata["FileName"];

                        if (!string.IsNullOrEmpty(cloudBlob.Metadata["UserId"]))
                        {
                            newContentBlob.Metadata["UserId"] = cloudBlob.Metadata["UserId"];
                        }
                        if (!string.IsNullOrEmpty(cloudBlob.Metadata["Published"]))
                        {
                            newContentBlob.Metadata["Published"] = cloudBlob.Metadata["Published"];
                        }
                        if (!string.IsNullOrEmpty(cloudBlob.Metadata["Size"]))
                        {
                            newContentBlob.Metadata["Size"] = cloudBlob.Metadata["Size"];
                        }
                        if (cloudBlob.Metadata.AllKeys.Contains("AlternateText"))
                        {
                            newContentBlob.Metadata["AlternateText"] = cloudBlob.Metadata["AlternateText"];
                        }
                        if (cloudBlob.Metadata.AllKeys.Contains("Description"))
                        {
                            newContentBlob.Metadata["Description"] = cloudBlob.Metadata["Description"];
                        }
                        if (cloudBlob.Metadata.AllKeys.Contains("Title"))
                        {
                            newContentBlob.Metadata["Title"] = cloudBlob.Metadata["Title"];
                        }

                        newContentBlob.SetMetadata();
                        cloudBlob.DeleteIfExists();
                    }
                }
            }
        }
    }
}

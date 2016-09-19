using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Datapecker.Agent.ReportingService;

namespace Datapecker.Agent
{
    internal class FileStorage
    {
        //поля
        protected FileInfo _fileInfo;
        protected ReaderWriterLockSlim _fileLocker;


        //инициализация
        public FileStorage(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            _fileLocker = new ReaderWriterLockSlim();
        }



        //single object
        public void CreateItem<T>(T item, out Exception exception)
        {
            exception = null;

            try
            {
                _fileLocker.EnterWriteLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(true);
                if (fileInfo.Exists)
                    fileInfo.Delete();
                
                using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Append
                    , FileAccess.Write, FileShare.ReadWrite))
                using (XmlWriter xw = XmlWriter.Create(fileStream))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(T));
                    ser.WriteObject(xw, item);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitWriteLock();
            }
        }

        public T ReadItem<T>(out Exception exception)
        {
            exception = null;
            T item = default(T);

            try
            {
                _fileLocker.EnterReadLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(false);
                if (!fileInfo.Exists || fileInfo.Length == 0)
                    return item;
                
                using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open
                   , FileAccess.Read, FileShare.ReadWrite))
                using (XmlReader xr = XmlReader.Create(fileStream))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(T));
                    item = (T)ser.ReadObject(xr);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitReadLock();
            }

            return item;
        }



        //line list
        public void CreateLineList<T>(List<T> lines, out Exception exception)
        {
            exception = null;

            try
            {
                _fileLocker.EnterWriteLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(true);
                if (fileInfo.Exists)
                    fileInfo.Delete();

                using (FileStream fileStream = new FileStream(fileInfo.FullName
                    , FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    foreach (T line in lines)
                    {
                        streamWriter.WriteLine(line.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitWriteLock();
            }
        }

        public void AppendToLineList<T>(List<T> lines, out Exception exception)
        {
            exception = null;

            try
            {
                _fileLocker.EnterWriteLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(true);

                using (FileStream fileStream = new FileStream(fileInfo.FullName
                    , FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    foreach (T line in lines)
                    {
                        streamWriter.WriteLine(line.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitWriteLock();
            }
        }

        public List<string> ReadLineList(out Exception exception)
        {
            return ReadLineList(int.MaxValue, out exception);
        }

        public List<string> ReadLineList(int count, out Exception exception)
        {
            List<string> list = new List<string>();
            exception = null;

            try
            {
                _fileLocker.EnterReadLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(false);
                if (!fileInfo.Exists || fileInfo.Length == 0)
                    return list;

                using (FileStream fileStream = new FileStream(fileInfo.FullName
                    , FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream && list.Count < count)
                    {
                        string line = streamReader.ReadLine();
                        list.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitReadLock();
            }

            return list;
        }

        public bool HasLineListContent(out Exception exception)
        {
            bool hasContent = false;
            exception = null;

            try
            {
                _fileLocker.EnterReadLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(false);
                if (!fileInfo.Exists || fileInfo.Length == 0)               
                    return false;                
                
                using (FileStream fileStream = new FileStream(fileInfo.FullName
                    , FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string line = streamReader.ReadLine();
                    hasContent = !string.IsNullOrEmpty(line);                   
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitReadLock();
            }

            return hasContent;
        }

        public void CutLineListFromStart(int count, out Exception exception)
        {
            exception = null;

            try
            {
                _fileLocker.EnterWriteLock();

                FileInfo originalFile = CreateDirectoryAndGetFile(true);
                if (!originalFile.Exists || originalFile.Length == 0)
                {
                    return;
                }

                FileInfo cutFile = new FileInfo(originalFile.FullName + ".cut");
                if (cutFile.Exists)
                {
                    cutFile.Delete();
                }

                CutLineList(count, cutFile);

                originalFile.Delete();
                cutFile.Refresh();
                if (cutFile.Exists)
                {
                    cutFile.MoveTo(originalFile.FullName);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitWriteLock();
            }
        }

        private void CutLineList(int count, FileInfo cutFile)
        {
            int index = 0;
            var formatter = new BinaryFormatter();

            using (var originalStream = new FileStream(_fileInfo.FullName, FileMode.Open
                , FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader streamReader = new StreamReader(originalStream))
            {
                while (!streamReader.EndOfStream && index < count)
                {
                    string line = streamReader.ReadLine();
                    index++;
                }

                if (!streamReader.EndOfStream)
                {
                    using (FileStream cutFileStream = new FileStream(cutFile.FullName, FileMode.CreateNew
                        , FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter streamWriter = new StreamWriter(cutFileStream))
                    {
                        char[] buffer = new char[4096];
                        while (true)
                        {
                            int readCount = streamReader.ReadBlock(buffer, 0, buffer.Length);
                            if (readCount <= 0)
                            {
                                break;
                            }
                            streamWriter.Write(buffer, 0, readCount);
                        }
                    }                    
                }
            }

        }


        
        //binary list
        public void AppendToBinaryList<T>(List<T> items, out Exception exception)
        {
            exception = null;

            try
            {
                _fileLocker.EnterWriteLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(true);

                using (var fileStream = File.Open(fileInfo.FullName
                    , FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    var formatter = new BinaryFormatter();
                    foreach (T item in items)
                    {
                        formatter.Serialize(fileStream, item);
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitWriteLock();
            }
        }

        public List<T> ReadBinaryList<T>(int count, out Exception exception)
        {
            List<T> list = new List<T>();
            exception = null;

            try
            {
                _fileLocker.EnterReadLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(false);
                if (!fileInfo.Exists || fileInfo.Length == 0)
                    return list;

                var formatter = new BinaryFormatter();

                using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open
                    , FileAccess.Read, FileShare.ReadWrite))
                {
                    while (fileStream.Position != fileStream.Length && list.Count < count)
                    {
                        list.Add((T)formatter.Deserialize(fileStream));
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitReadLock();
            }

            return list;
        }

        public bool HasBinaryListContent<T>(out Exception exception)
        {
            bool hasContent = false;
            exception = null;

            try
            {
                _fileLocker.EnterReadLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(false);
                if (!fileInfo.Exists || fileInfo.Length == 0)                
                    return false;

                var formatter = new BinaryFormatter();

                using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open
                    , FileAccess.Read, FileShare.ReadWrite))
                {                   
                    T firstValue = (T)formatter.Deserialize(fileStream);
                    hasContent = !EqualityComparer<T>.Default.Equals(firstValue, default(T));                    
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitReadLock();
            }

            return hasContent;
        }

        public void CutBinaryListFromStart(int count, out Exception exception)
        {
            exception = null;

            try
            {
                _fileLocker.EnterWriteLock();

                FileInfo originalFile = CreateDirectoryAndGetFile(true);
                if (!originalFile.Exists || originalFile.Length == 0)
                { 
                    return;
                }

                FileInfo cutFile = new FileInfo(originalFile.FullName + ".cut");
                if (cutFile.Exists)
                {
                    cutFile.Delete();
                }

                CutBinaryList(count, cutFile);

                originalFile.Delete();
                cutFile.Refresh();
                if (cutFile.Exists)
                {
                    cutFile.MoveTo(originalFile.FullName);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitWriteLock();
            }
        }

        private void CutBinaryList(int count, FileInfo cutFile)
        {
            int index = 0;
            var formatter = new BinaryFormatter();

            using (var originalStream = new FileStream(_fileInfo.FullName, FileMode.Open
                , FileAccess.Read, FileShare.ReadWrite))
            {
                while (originalStream.Position != originalStream.Length && index < count)
                {
                    formatter.Deserialize(originalStream);
                    index++;
                }

                if (originalStream.Position != originalStream.Length)
                {
                    using (var cutFileStream = new FileStream(cutFile.FullName, FileMode.CreateNew
                        , FileAccess.Write, FileShare.ReadWrite))
                    {
                        originalStream.CopyTo(cutFileStream);
                    }
                }
            }
        }



        //ExceptionEvent binary list
        public List<string> ReadDistinctExceptionEventKeys(out Exception exception)
        {
            List<string> list = new List<string>();
            exception = null;

            try
            {
                _fileLocker.EnterReadLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(false);
                if (!fileInfo.Exists || fileInfo.Length == 0)
                    return list;

                var formatter = new BinaryFormatter();

                using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open
                    , FileAccess.Read, FileShare.ReadWrite))
                {
                    while (fileStream.Position != fileStream.Length)
                    {
                        var item = (ExceptionEvent)formatter.Deserialize(fileStream);
                        list.Add(item.EventKey);
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitReadLock();
            }

            return list.Distinct().ToList();
        }
        


        //удаление
        public void DeleteFile(out Exception exception)
        {
            exception = null;

            try
            {
                _fileLocker.EnterWriteLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(false);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _fileLocker.ExitWriteLock();
            }
        }

        public void MoveDamaged(out Exception exception)
        {
            exception = null;

            try
            {
                _fileLocker.EnterWriteLock();

                FileInfo fileInfo = CreateDirectoryAndGetFile(false);
                if (!fileInfo.Exists)
                {
                    return;
                }

                string destination = fileInfo.FullName + ".damaged";
                FileInfo destinationFileInfo = new FileInfo(destination);
                if(destinationFileInfo.Exists)
                {
                    destinationFileInfo.Delete();
                }

                fileInfo.MoveTo(destination);
            }
            catch (Exception ex)
            {
                exception = exception ?? ex;
            }
            finally
            {
                _fileLocker.ExitWriteLock();
            }
        }



        //общее
        protected FileInfo CreateDirectoryAndGetFile(bool createDirectory)
        {
            _fileInfo.Directory.Refresh();           
            if (!_fileInfo.Directory.Exists && createDirectory)
            {
                _fileInfo.Directory.Create();
            }
            
            return new FileInfo(_fileInfo.FullName);
        }      
    }
}

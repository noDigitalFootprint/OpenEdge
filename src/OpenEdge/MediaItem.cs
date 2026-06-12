using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using OpenEdge; // For MediaKind

namespace OpenEdge;

public class MediaItem : INotifyPropertyChanged
{
    private BitmapSource thumbnail;

    public string FullPath { get; }
    public string RelativePath { get; }
    public string FileName { get; }
    public MediaKind Kind { get; }
    public string SourceId { get; }
    public long SizeBytes { get; }
    public DateTime LastWriteUtc { get; }
    public BitmapSource Thumbnail
    {
        get => thumbnail;
        set
        {
            if (!ReferenceEquals(thumbnail, value))
            {
                thumbnail = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    
    // For display in UI
    public string KindString => Kind.ToString();
    public string SizeString => FormatFileSize(SizeBytes);

    public MediaItem(string fullPath, string relativePath, string fileName, MediaKind kind, 
                     string sourceId, long sizeBytes, DateTime lastWriteUtc)
    {
        FullPath = fullPath;
        RelativePath = relativePath;
        FileName = fileName;
        Kind = kind;
        SourceId = sourceId;
        SizeBytes = sizeBytes;
        LastWriteUtc = lastWriteUtc;
    }

    private string FormatFileSize(long bytes)
    {
        string[] suffix = { "B", "KB", "MB", "GB", "TB" };
        int i = 0;
        double dblSBytes = bytes;
        
        if (bytes == 0)
            return "0" + suffix[0];
            
        while (dblSBytes >= 1024 && i < suffix.Length - 1)
        {
            dblSBytes /= 1024;
            i++;
        }
        
        return string.Format("{0:0.##} {1}", dblSBytes, suffix[i]);
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

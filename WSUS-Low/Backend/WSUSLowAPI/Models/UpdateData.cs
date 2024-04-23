namespace WSUSLowAPI.Models
{
    public class UpdateData
    {
        public int Id { get; set; }
        public Guid? UpdateID { get; set; }
        public int? RevisionNumber { get; set; }
        public string? DefaultPropertiesLanguage { get; set; }
        public string? UpdateType { get; set; }
        public long? MaxDownloadSize { get; set; }
        public long? MinDownloadSize { get; set; }
        public string? PublicationState { get; set; }
        public DateTime? CreationDate { get; set; }
        public Guid? PublisherID { get; set; }
        public string? Title { get; set; }

        public void ValidateUpdateID()
        {
            throw new NotImplementedException();
        }

        public void ValidateRevisionNumber()
        {
            throw new NotImplementedException();
        }
        public void ValidateDefaultPropertiesLanguage()
        {
            throw new NotImplementedException();
        }
        public void ValidateUpdateType()
        {
            throw new NotImplementedException();
        }
        public void ValidateMaxDownloadSize()
        {
            throw new NotImplementedException();
        }
        public void ValidateMinDownloadSize()
        {
            throw new NotImplementedException();
        }
        public void ValidatePublicationState()
        {
            throw new NotImplementedException();
        }
        public void ValidateCreationDate()
        {
            throw new NotImplementedException();
        }
        public void ValidatePublisherID()
        {
            throw new NotImplementedException();
        }
        public void ValidateTitle()
        {
            throw new NotImplementedException();
        }
        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}

namespace YasamPsikologProject.EntityLayer.Enums
{
    public enum AppointmentStatus
    {
        Pending = 1,        // Beklemede
        Confirmed = 2,      // Onaylandı
        Completed = 3,      // Tamamlandı
        Cancelled = 4,      // İptal Edildi
        NoShow = 5          // Gelmedi
    }
}

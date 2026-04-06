namespace Amigos.Models
{
    public class AmigoDistanciaViewModel
    {
        public List<Amigo>? Amigos { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public double? DistanciaMaxKm { get; set; }
    }
}

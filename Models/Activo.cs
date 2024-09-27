namespace OrdenesInversion.Models
{
    public class Activo
    {
        public int id { get; set; }
        public string ticker { get; set; }
        public string nombre { get; set; }
        public int tipoActivo { get; set; }
        public decimal precioUnitario { get; set; }
    }
}

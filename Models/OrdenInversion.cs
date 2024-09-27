using Newtonsoft.Json;

namespace OrdenesInversion.Models
{
    public class OrdenInversionRequest
    {
        public int idCuenta { get; set; }
        public string nombreActivo { get; set; }
        public int cantidad { get; set; }
        public decimal precio { get; set; }
        public string operacion { get; set; }
    }

    public class OrdenInversion
    {
        public int idOrden { get; set; }
        public int idCuenta { get; set; }
        public string nombreActivo { get; set; }
        public int cantidad { get; set; }
        public decimal precio { get; set; }
        public string operacion { get; set; }
        public int? estado { get; set; }
        public decimal? montoTotal { get; set; }

        public override string ToString()
        {
            return ($"{idOrden},{idCuenta},{nombreActivo},{cantidad},{precio},{operacion},{estado},{montoTotal}");
        }
    }

    public enum OrdenParamsValid
    {
        None = 0,
        NombreActivo = 1,
        Cantidad = 2,
        Precio = 3,
        Operacion = 4,
        Estado = 5,
        MontoTotal = 6
    }

    public enum OrdenEstados
    {
        EnProceso = 0,
        Ejecutada = 1,
        Cancelada = 3
    }

    public enum TiposActivos
    {
        Accion = 1,
        Bono = 2,
        FCI = 3
    }
}

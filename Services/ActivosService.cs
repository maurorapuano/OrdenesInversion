using OrdenesInversion.Models;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using System;


namespace OrdenesInversion.Services
{
    public class ActivosService
    {
        List<Activo> _activos;

        public ActivosService()
        {
            try
            {
                string rutaBase = Directory.GetCurrentDirectory();
                string activosPath = Path.Combine(rutaBase, "DatosOrdenes");
                Directory.CreateDirectory(activosPath);
                activosPath = Path.Combine(activosPath, "Activos.csv");

                using (var reader = new StreamReader(activosPath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    _activos = new List<Activo>(csv.GetRecords<Activo>());
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public Activo getActivo(string nombre)
        {
            return _activos.Find(a => a.nombre.ToLower() == nombre.ToLower());
        }

        public decimal ApplyComisionesImpuestos(decimal montoTotal, decimal porcComisiones, decimal porcImpuestos)
        {
            decimal comisiones = montoTotal * porcComisiones;
            decimal impuestos = comisiones * porcImpuestos;

            montoTotal += comisiones;
            montoTotal += impuestos;

            return montoTotal;
        }
    }
}

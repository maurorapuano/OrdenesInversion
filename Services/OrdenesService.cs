using CsvHelper;
using OrdenesInversion.Models;
using System.Globalization;

namespace OrdenesInversion.Services
{
    public class OrdenesService
    {
        public List<OrdenInversion> _ordenes;
        public ActivosService _activosService;
        private readonly string filePath;
        public OrdenesService(ActivosService activosService) 
        {
            _activosService = activosService;

            try
            {
                /* Implemento BBDD usando un archivo CSV */
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "DatosOrdenes");
                Directory.CreateDirectory(filePath);
                filePath = Path.Combine(filePath, "Ordenes-Inversion.csv");

                if (!File.Exists(filePath))
                    if (!createOrdenesFile(filePath)) throw new Exception("Error creando archivo de base de datos.");

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    _ordenes = new List<OrdenInversion>(csv.GetRecords<OrdenInversion>());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Dictionary<string,string> validateParamsOrden(OrdenInversionRequest orden)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (orden.nombreActivo.Length > 32)
                result.Add("nombreActivo", "Largo máximo de 32 caracteres.");

            if (orden.cantidad <= 0)
                result.Add("cantidad", "Debe ser mayor que 0.");

            if (orden.precio <= 0)
                result.Add("precio", "Debe ser mayor que 0.");

            if(orden.operacion.ToLower() != "c" && orden.operacion.ToLower() != "v")
                result.Add("operacion", "'C' para indicar Compra, 'V' para indicar Venta.");

            return result;
        }

        public bool addOrden(OrdenInversionRequest orden)
        {
            OrdenInversion newOrden = new OrdenInversion();
            bool result = false;
            decimal montoTotal = 0;

            montoTotal = calculateMontoTotal(orden);

            if (montoTotal <= 0)
                throw new Exception("Error al calcular Monto Total.");

            string rutaBase = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(rutaBase, "DatosOrdenes");
            filePath = Path.Combine(filePath, "Ordenes-Inversion.csv");

            newOrden.idOrden = _ordenes.Count + 1;
            newOrden.idCuenta = orden.idCuenta;
            newOrden.nombreActivo = orden.nombreActivo;
            newOrden.cantidad = orden.cantidad;
            newOrden.precio = orden.precio;
            newOrden.operacion = orden.operacion;
            newOrden.estado = (int)OrdenEstados.EnProceso;
            newOrden.montoTotal = montoTotal;


            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(newOrden.ToString());
            }

            result = true;

            return result;
        }

        private bool createOrdenesFile(string filePath)
        {
            /* En primera pasada, creo archivo para indicarle cabeceras */
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("idOrden,idCuenta,nombreActivo,cantidad,precio,operacion,estado,montoTotal");
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private decimal calculateMontoTotal(OrdenInversionRequest orden)
        {
            decimal montoTotal = 0;
            Activo activo = _activosService.getActivo(orden.nombreActivo);
            if (activo == null)
                throw new Exception("Error al buscar Activo.");

            /* Dudas con respecto a 'No debe recibir precio' y diferencia con 'Recibe precio'
             * Imagino tiene que ver con el parametro del metodo, pero uso el precio unitario en los tres casos */
            montoTotal = activo.precioUnitario * orden.cantidad;

            switch (activo.tipoActivo)
            {
                case (int)TiposActivos.Accion:
                    montoTotal = _activosService.ApplyComisionesImpuestos(montoTotal, (decimal)0.006, (decimal)0.21);
                    break;
                case (int)TiposActivos.Bono:
                    montoTotal = _activosService.ApplyComisionesImpuestos(montoTotal, (decimal)0.002, (decimal)0.21);
                    break;
            }
                        
            return montoTotal;
        }

        public OrdenInversion GetOrden(int idOrden)
        {
            return _ordenes.Find(o => o.idOrden == idOrden);
        }

        public bool UpdateOrden(int idOrden, int estado)
        {
            bool status = false;

            OrdenInversion orden = _ordenes.FirstOrDefault(o => o.idOrden == idOrden);

            if (orden == null)
                return status;

            orden.estado = estado;

            /* Sobreescribo el archivo completo con el registro actualizado */
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("idOrden,idCuenta,nombreActivo,cantidad,precio,operacion,estado,montoTotal");
                _ordenes.ForEach(o => {
                    writer.WriteLine(o.ToString());
                });                
            }
            status = true;

            return status;
        }
        public bool DeleteOrden(int idOrden)
        {
            bool status = false;

            OrdenInversion orden = _ordenes.FirstOrDefault(o => o.idOrden == idOrden);

            if (orden == null)
                return status;

            _ordenes.Remove(orden);

            /* Sobreescribo el archivo completo con el registro actualizado */
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("idOrden,idCuenta,nombreActivo,cantidad,precio,operacion,estado,montoTotal");
                _ordenes.ForEach(o => {
                    writer.WriteLine(o.ToString());
                });
            }
            status = true;

            return status;
        }
    }
}

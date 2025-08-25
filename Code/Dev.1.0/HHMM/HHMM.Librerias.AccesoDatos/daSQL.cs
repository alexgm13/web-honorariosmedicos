using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using HHMM.Librerias.EntidadesNegocio;
using General.Librerias.EntidadesNegocio;

namespace HHMM.Librerias.AccesoDatos
{
    public class daSQL
    {
		public string EjecutarComando(SqlConnection con, string nombreProcedimiento,
			string nombreParametro = null, string valorParametro = null)
		{
			string response = "";

			SqlCommand cmd = new SqlCommand(nombreProcedimiento, con);
			cmd.CommandType = CommandType.StoredProcedure;

			if (nombreParametro != null && valorParametro != null)
			{
				cmd.Parameters.AddWithValue(nombreParametro, valorParametro);
			}
			object data = cmd.ExecuteScalar();
			if (data != null)
			{
				response = data.ToString();
			}
			return response;
		}
        private static readonly HashSet<string> ProcedimientosPermitidos = new HashSet<string>
        {
            "uspReporteCTACTEDescargarV2",
            "uspReporteDetalladoListarV5",
            "uspCuentaCorrienteMedicoContableExportar"
        };
        public DataSet EjecutarComandoDst(SqlConnection con, string NombreSP, string parametroNombre = "", string parametroValor = "")
        {
            if (!ProcedimientosPermitidos.Contains(NombreSP))
            {
                throw new InvalidOperationException($"El procedimiento {NombreSP} no está permitido.");
            }

            var result = new DataSet();

            using (var adapter = new SqlDataAdapter())
            {
                adapter.SelectCommand = new SqlCommand(NombreSP, con)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 0
                };

                if (!String.IsNullOrEmpty(parametroNombre) && !String.IsNullOrEmpty(parametroValor))
                {
                    var parameter = new SqlParameter(parametroNombre, parametroValor);
                    adapter.SelectCommand.Parameters.Add(parameter);
                }

                adapter.Fill(result);
            }
            return result;
        }
	}
}

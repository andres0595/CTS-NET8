using CTS_NET8.Models;

namespace CTS_NET8.Configurations
{
    public class SqlController
    {
        private string[] comparators = new string[2] { "=", ">" };
        string statement = string.Empty;
        public string ActualizarTabla(QuerySqlViewModel info)
        {
            return ConstruirActualizacion(info);
        }

        public string Selectotable(QuerySqlViewModel info)
        {
            return BuildSelectotable(info);
        }

        #region Obtener Inner join
        public string InnerJoin(QuerySqlViewModel info)
        {
            return ArmSelectInnerJoin(info);
        }
        #endregion

        #region Obtener Delete
        public string Eliminacion(QuerySqlViewModel info)
        {
            return ConstruirEliminacion(info);
        }
        #endregion

        #region Obtener sentencia INSERT
        public string Insert(QuerySqlViewModel info)
        {
            return ArmInsert(info);
        }
        #endregion

        #region Obtener sentencia INSERT CON DAPPER
        public string InsertDapper(QuerySqlViewModel info)
        {
            return ArmInsertDapper(info);
        }
        #endregion

        #region Obteber sentencia select in
        public string selectIn(QuerySqlViewModel info)
        {
            return ArmSelectIn(info);
        }
        #endregion

        #region Obtener Inner join
        public string ConsultarExistencias()
        {
            return ValidadorSolicitud();
        }
        #endregion

        private string ArmSelectInnerJoin(QuerySqlViewModel info)
        {
            statement = "SELECT ";

            for (int i = 0; i < info.valores.Length; i++)
            {
                if (i == (info.valores.Length - 1))
                {
                    statement += info.valores[i] + " ";
                }
                else
                {
                    statement += info.valores[i] + ", ";
                }
            }

            //statement += "FROM "+ info.tablas[0] + " " + info.idenTabla[0] + " INNER JOIN " +
            //    info.tablas[1] + " " + info.idenTabla[1] + " ON " + " " + info.idenTabla[0] + "." + info.join[0] + "=" + info.idenTabla[1] + "." + info.join[1]
            //    +" INNER JOIN "+ info.tablas[2] + " " + info.idenTabla[2] + " ON "+" "+ info.idenTabla[2]+"."+ info.join[2]+"=" + info.idenTabla[1]+"."+ info.join[3];


            statement += "FROM ";


            for (int i = 0; i < info.tablas.Length; i++)
            {
                if (i == (info.tablas.Length - 1))
                {
                    if (info.igualador == 0)
                    {
                        var constante = info.eval == 0 ? info.join[i + 1] : info.join[i];
                        statement += info.tablas[i] + " " + info.idenTabla[i] + " ON " + " " + info.idenTabla[i] + "." + info.join[i] + "=" + info.idenTabla[i - 1] + "." + constante;
                    }
                    else
                    {
                        statement += info.tablas[i] + " " + info.idenTabla[i] + " ON " + " " + info.idenTabla[i] + "." + info.join[i] + "=" + info.idenTabla[i] + "." + info.join[i];
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        statement += info.tablas[i] + " " + info.idenTabla[i] + " INNER JOIN ";
                    }
                    else
                    {
                        statement += info.tablas[i] + " " + info.idenTabla[i] + " ON " + " " + info.idenTabla[i] + "." + info.join[i] + "=" + info.idenTabla[i - 1] + "." + info.join[i - 1] + " INNER JOIN ";
                    }
                }
            }


            if (info.condiciones != null)
            {
                statement += " WHERE ";
                for (int i = 0; i < info.condiciones.Length; i++)
                {
                    if (i == (info.condiciones.Length - 1))
                    {
                        if (info.typeCondition != null) { statement += info.idenTabla[info.eval] + "." + info.condiciones[i] + comparators[info.typeCondition[i]] + "@" + info.condiciones[i] + ";"; }

                        else { statement += info.idenTabla[0] + "." + info.condiciones[i] + "=@" + info.condiciones[i] + ";"; }
                    }
                    else
                    {
                        if (info.typeCondition != null) { statement += info.idenTabla[info.eval] + "." + info.condiciones[i] + comparators[info.typeCondition[i]] + "@" + info.condiciones[i] + " AND "; }

                        else { statement += info.idenTabla[info.eval] + "." + info.condiciones[i] + "=@" + info.condiciones[i] + " AND "; }
                    }

                }
            }

            return statement;
        }

        private string BuildSelectotable(QuerySqlViewModel info)
        {
            statement = "SELECT ";

            for (int i = 0; i < info.valores.Length; i++)
            {
                if (i == (info.valores.Length - 1))
                {
                    statement += info.valores[i] + " ";
                }
                else
                {
                    statement += info.valores[i] + ", ";
                }
            }

            statement += "FROM " + info.tabla;

            if (info.condiciones != null)
            {
                statement += " WHERE ";
                for (int i = 0; i < info.condiciones.Length; i++)
                {
                    if (i == (info.condiciones.Length - 1))
                    {
                        statement += info.condiciones[i] + "=@" + info.condiciones[i] + ";";
                    }
                    else
                    {
                        statement += info.condiciones[i] + "=@" + info.condiciones[i] + " AND ";
                    }

                }
            }
            return statement;
        }

        private string ConstruirActualizacion(QuerySqlViewModel info)
        {
            statement = "UPDATE " + info.tabla + " SET ";

            for (int i = 0; i < info.valores.Length; i++)
            {
                if (i == (info.valores.Length - 1))
                {
                    statement += info.valores[i] + "=@" + info.valores[i] + " ";
                }
                else
                {
                    statement += info.valores[i] + "=@" + info.valores[i] + ", ";
                }
            }

            statement += "WHERE ";


            for (int i = 0; i < info.condiciones.Length; i++)
            {
                if (i == (info.condiciones.Length - 1))
                {
                    statement += info.condiciones[i] + "=@" + info.condiciones[i] + ";";
                }
                else
                {
                    statement += info.condiciones[i] + "=@" + info.condiciones[i] + " AND ";
                }

            }


            return statement;
        }

        private string ArmInsert(QuerySqlViewModel info)
        {
            statement = "INSERT INTO " + info.tabla + " VALUES( ";
            for (int i = 0; i < info.inserts.Length; i++)
            {
                if (i == (info.inserts.Length - 1))
                {

                    if (info.inserts[i] == "") { statement += "NULL)"; } else { statement += "'" + info.inserts[i] + "')"; }
                }
                else
                {

                    if (info.inserts[i] == "") { statement += "NULL,"; } else { statement += "'" + info.inserts[i] + "',"; }
                }
            }

            return statement;
        }

        private string ArmInsertDapper(QuerySqlViewModel info)
        {
            string statement = "INSERT INTO " + info.tabla + " VALUES( ";
            for (int i = 0; i < info.inserts.Length; i++)
            {
                if (i == (info.inserts.Length - 1))
                {

                    if (info.inserts[i] == "") { statement += "NULL)"; } else { statement += "@" + info.inserts[i] + ")"; }
                }
                else
                {

                    if (info.inserts[i] == "") { statement += "NULL,"; } else { statement += "@" + info.inserts[i] + ","; }
                }
            }

            return statement;
        }

        private string ConstruirEliminacion(QuerySqlViewModel info)
        {
            statement = "DELETE " + info.tabla;

            statement += "  WHERE ";

            for (int i = 0; i < info.condiciones.Length; i++)
            {
                if (i == (info.condiciones.Length - 1))
                {
                    statement += info.condiciones[i] + "=@" + info.condiciones[i] + ";";
                }
                else
                {
                    statement += info.condiciones[i] + "=@" + info.condiciones[i] + " AND ";
                }

            }
            return statement;
        }

        #region Armar sentencia Select IN
        private string ArmSelectIn(QuerySqlViewModel info)
        {
            string statement = "SELECT ";

            for (int i = 0; i < info.valores.Length; i++)
            {
                if (i == (info.valores.Length - 1))
                {
                    statement += info.valores[i] + " ";
                }
                else
                {
                    statement += info.valores[i] + ", ";
                }
            }

            statement += "FROM " + info.tabla;

            if (info.condiciones != null)
            {
                statement += " WHERE " + info.join[0] + " IN (";

                for (int i = 0; i < info.condiciones.Length; i++)
                {
                    if (i == (info.condiciones.Length - 1))
                    {
                        if (info.types[0] == 0)
                        {
                            statement += info.condiciones[i] + ")";
                        }
                        else
                        {
                            statement += "'" + info.condiciones[i] + "'" + ")";
                        }
                    }
                    else
                    {
                        if (info.types[0] == 0)
                        {
                            statement += info.condiciones[i] + ",";
                        }
                        else
                        {
                            statement += "'" + info.condiciones[i] + "'" + ",";
                        }
                    }
                }
            }



            return statement;
        }
        #endregion

        public string InnerJoinStatic(string bandera, int filtro, int IdSolicitud, int IdVenta)
        {
            return BuildInnerJoinStatic(bandera, filtro, IdSolicitud, IdVenta);
        }

        private string BuildInnerJoinStatic(string bandera, int filtro, int IdSolicitud, int idVenta)
        {
            statement = string.Empty;
            if (bandera == "P")
            {
                statement = "SELECT Examenes.CodAthenea as Idprocedimiento, Examenes.CodAthenea + ' - ' + Examenes.NombreExamen as NombreProcedimiento," +
                     "CASE WHEN EXISTS(SELECT IdExcepcionDetal FROM ExcepcionesPlan_Detalle ED INNER JOIN ExcepcionesPlan EP ON EP.IdExcepcion = ED.IdExcepcion" +
                       " WHERE IdProcedimiento = Examenes.IdExamen AND EP.IdPlan = 1 AND ED.Activo = 1 AND Tipo_Procedimiento = 'EX')" +
                       " THEN(SELECT Valor_Excepcion FROM ExcepcionesPlan_Detalle ED INNER JOIN ExcepcionesPlan EP ON EP.IdExcepcion = ED.IdExcepcion " +
                           " WHERE IdProcedimiento = Examenes.IdExamen AND EP.IdPlan = 1 AND ED.Activo = 1 AND Tipo_Procedimiento = 'EX') ELSE " +
               "E.Precio END as ValorTarifa," +
               "0 as IdCategoria," +
               "Examenes.Observaciones AS Recomendaciones, Examenes.Req_Domicilio,Examenes.Visualizar_Web,Examenes.IdExamen AS Identificador, " +
               "Contenido_Html, Examenes.Descripcion, Examenes.CUPS " +
               "FROM Examenes INNER JOIN " +
               "EsquemaTarifario E ON Examenes.IdExamen = E.IdProcedimiento AND E.CatProcedimiento = 'EX' INNER JOIN " +
               "Tarifas ON E.IdTarifa = Tarifas.IdTarifa LEFT JOIN " +
               "Planes ON Tarifas.IdTarifa = Planes.IdTarifa WHERE Examenes.Activo=1 AND (NombreExamen LIKE '%PATERNIDAD%' OR NombreExamen LIKE '%MATERNIDAD%' OR NombreExamen LIKE '%PERFIL GENÉTICO%') AND Planes.IdPlan=1";
            }
            else
            {
                statement = "SELECT A.idPaciente, A.IdAgendamiento, CASE WHEN A.AgendaColcan=1 THEN A.SedeAgendamiento ELSE A.IdAgenda END AS IdAgenda, P.Primer_Nombre + ' ' + P.Segundo_Nombre + ' ' + P.Primer_Apellido + ' ' + P.Segundo_Apellido AS Nombres, " +
                            "A.FechaAgendamiento,[dbo].[F_Retorna_LugarAgendamiento](A.IdAgendamiento) AS Lugar, C.Nombre AS Ciudad,CASE WHEN A.AgendaColcan=1 THEN 'Sede Colcan' ELSE T.TipoServicio END AS TipoServicio,CASE WHEN A.AgendaColcan=1 THEN 9999 ELSE AG.IdTipoServ END AS IdTipoServ, " +
                            "PR.Parentesco,A.FechaAgendamiento, " +
                            "(SELECT TOP 1 DA.IdDiasAten FROM Agenda_Horarios AH LEFT JOIN Dias D ON D.IdDia = AH.IdDia LEFT JOIN DiasAtencion DA ON DA.IdDiasAten = D.IdDiasAten WHERE AH.IdAgendaDetal = AD.IdAgendaDetal)AS IdDiasAten, D.IdDepartamento " +
                "FROM Agendamiento A INNER JOIN Paciente P ON A.IdPaciente = P.Id_Paciente LEFT JOIN Ciudades C ON C.IdCiudad = P.IdCiudad " +
                "LEFT JOIN Agenda_Detalle AD ON AD.IdAgendaDetal = A.IdAgenda " +
                "LEFT JOIN Agenda AG ON AG.IdAgenda = AD.IdAgenda " +
                "LEFT JOIN TipoServicio T ON AG.IdTipoServ = T.IdTipoServ " +
                "INNER JOIN GruposPacientes GP ON GP.IdPaciente=P.Id_Paciente " +
                "LEFT JOIN Parentesco PR ON GP.IdParentesco = PR.IdParentesco " +
                "LEFT JOIN Sedes_Colcan SC ON SC.IdSede=A.SedeAgendamiento " +
                "LEFT JOIN Departamentos D ON D.IdDepartamento = SC.IdDepartamento " +
                "WHERE GP.IdSolicitud=" + IdSolicitud + " AND ((" + idVenta + "!=0 AND A.IdVenta =" + idVenta + ") OR (" + idVenta + "=0 AND A.IdVenta IS NULL)) ";
            }
            return statement;
        }

        public string ConsultaDetallePaternidad(string proceso)
        {
            return ConsultDetallPaternidad(proceso);
        }



        public string consultaInfoProcedimiento(string documento, int idTipodoc)
        {
            return InfoUltimpoProcedimiento(documento, idTipodoc);
        }

        public string consultaReportes()
        {
            return reporteEncuesta();
        }

        public string detalleEncuesta()
        {
            return detailEncuesta();
        }


        private string ConsultDetallPaternidad(string proceso)
        {
            string statement = string.Empty;
            {
                if (proceso == "V")
                {
                    statement = "SELECT V.IdEstadoVenta as IdEstado, SP.IdSolicitud,  " +
                    "V.IdSede, V.IdPlan, V.FechaVenta,E.IdExamen, SP.CantidadPacientes, sp.IdSolicitud_Lis as idRequestLis, s.IdAthenea as IdSedeAthenea, SLH.IdSedeLIS AS IdSedeLis " +
                    "FROM SolicitudesPaternidad sp JOIN Control_Venta_Paternidad cp on sp.IdSolicitud = cp.IdSolicitudPaternidad " +
                    "JOIN Ventas V ON V.IdVenta = CP.IdVenta JOIN Examenes E ON E.codAthenea = CONVERT(nvarchar, SP.IdProcedimiento) JOIN Sedes s on s.IdSede = v.IdSede " +
                    "JOIN Sedes_Lis_Homologacion SLH ON SLH.IdSedeCTS = V.IdSede WHERE V.IdVenta = @IdVenta " +
                   "SELECT [dbo].[F_Retorna_MediosPago_Solicitudes](V.IdVenta,'VP',209) as medioPago, " +
                   "V.IdMedioPago_Alterno as IdMedioPagoAlterno, V.idMedioPago as idMediosPago,  V.TotalVenta, VD.PrecioVenta " +
                   "FROM SolicitudesPaternidad sp JOIN Control_Venta_Paternidad cp on sp.IdSolicitud = cp.IdSolicitudPaternidad  " +
                   "JOIN Ventas V ON V.IdVenta = CP.IdVenta JOIN Ventas_Detalle VD ON VD.IdVenta = V.IdVenta  WHERE V.IdVenta = @IdVenta";
                }
                else if (proceso == "A") //Asociar el idVenta a los demas pacientes que no son titular
                {
                    statement = "UPDATE Agendamiento SET IdVenta = @IdVenta, IdEstado = 4 FROM GruposPacientes GP JOIN Agendamiento A ON A.IdPaciente = GP.IdPaciente " +
                        "JOIN SolicitudesPaternidad SP ON SP.IdSolicitud = GP.IdSolicitud JOIN Control_Venta_Paternidad CP ON SP.IdSolicitud = CP.IdSolicitudPaternidad " +
                        "WHERE GP.IdSolicitud = @IdPreSolicitud AND A.IdVenta IS NULL ";
                }
                else if (proceso == "T")//Validar si el titular de la venta esta asociado como titular a otra venta
                {
                    statement = "SELECT IdTransaccion FROM SolicitudesPaternidad sp JOIN Control_Venta_Paternidad cp on sp.IdSolicitud = cp.IdSolicitudPaternidad " +
                                 "JOIN Ventas V ON V.IdVenta = CP.IdVenta WHERE sp.IdSolicitud <> @idSolicitud AND IdEstadoVenta IN (1) AND IdTipoServ=219 AND  Id_Paciente = @idPaciente AND NOT EXISTS(SELECT IdRegistro FROM Ventas_Potenciales_Control_Presolicitud C WHERE V.IdVenta=C.IdVenta)";
                }
                else if (proceso == "P")//consulta el numero de presolicitud  de acuerdo a la fecha
                {
                    statement = "SELECT COUNT(*) FROM PreSolicitudes WHERE CONVERT(DATE, FechaSolicitud)= CONVERT(DATE, @FechaSolicitud)";
                }
                return statement;
            }
        }


        public string ConstruirDetalleExcelEncuesta()
        {
            return DetalleExcelEncuesta();
        }

        private string InfoUltimpoProcedimiento(string documento, int idTipodoc)
        {

            statement = "SELECT TOP 1 V.IdVenta, P.Id_Paciente," +
               "P.Primer_Nombre+' '+ CASE WHEN P.Segundo_Nombre IS NULL THEN '' ELSE P.Segundo_Nombre END+' '+P.Primer_Apellido+' '+ CASE WHEN P.Segundo_Apellido IS NULL THEN '' ELSE P.Segundo_Apellido END AS NombreCompleto," +
               " CASE WHEN [dbo].[F_Retorna_LugarAgendamiento_Venta_Potencial](V.IdVenta,'N')IS NULL THEN  S.NombreSede ELSE [dbo].[F_Retorna_LugarAgendamiento_Venta_Potencial](V.IdVenta,'N')END AS Sede_Atencion," +
               " [dbo].[F_Retorna_Categoria_Procedimientos](V.IdVenta) AS Tipos_Procedimientos," +
               " CASE WHEN [dbo].[F_Retorna_LugarAgendamiento_Venta_Potencial](V.IdVenta,'C')IS NULL THEN S.IdSede ELSE [dbo].[F_Retorna_LugarAgendamiento_Venta_Potencial](V.IdVenta,'I') END AS Sede_AtencionId," +
               "COALESCE((SELECT TOP 1 T.TipoServicio FROM Agendamiento AA INNER JOIN Agenda_Detalle AD ON AA.IdAgenda=AD.IdAgendaDetal INNER JOIN Agenda AG ON AG.IdAgenda=AD.IdAgenda " +
               "INNER JOIN TipoServicio T ON T.IdTipoServ=AG.IdTipoServ WHERE AA.IdVenta=A.IdVenta), T.TipoServicio ) AS Modelo_Atencion " +

               "FROM Paciente P INNER JOIN Ventas V ON V.Id_Paciente=P.Id_Paciente  LEFT JOIN Agendamiento A ON A.IdVenta=V.IdVenta INNER JOIN Sedes S ON S.IdSede=V.IdSede INNER JOIN TipoServicio T ON T.IdTipoServ=V.IdTipoServ " +
               "WHERE P.Documento='" + documento + "' AND P.IdTipoDocumento=" + idTipodoc +
                " AND V.IdEstadoVenta=2 AND V.NoFactura IS NOT NULL AND NOT EXISTS(SELECT 1 FROM EncuestaSatisfaccion E WHERE V.IdVenta=E.IdVenta) AND (A.IdAgendamiento IS NULL OR EXISTS (SELECT 1 FROM Agendamiento WHERE IdPaciente = P.Id_Paciente)) ORDER BY A.IdAgendamiento DESC";

            return statement;
        }

        private string reporteEncuesta()
        {
            statement = @"SELECT E.IdEncuesta, T.Abreviatura+' '+E.Documento as documento,	E.Nombres, S.NombreSede as sedeAtencion, E.segumiento as seguimiento, " +
             "UPPER(O.Opcion) AS Clasificacion, E.IdEstado," +
             "CONVERT(VARCHAR(20),E.FechaRegistro,103) AS FechaRegistro, CASE WHEN E.IdVenta=0 THEN E.Contacto ELSE P.Telefono END AS Telefono, CASE WHEN P.Correo IS NULL THEN '--' ELSE P.Correo END AS Correo," +
             "(SELECT [dbo].[F_Retorna_Edad_Pacientes](P.Fecha_Nacimiento)) AS Edad, ET.Nombre AS Estrato, L.NombreLocalidad,[dbo].[F_Retorna_Procedimientos_Venta_Potencial](E.IdVenta,1,'N',0) AS Nombre_procedimiento, " +
             "CASE WHEN E.IdVenta=0  THEN TP.Tipo ELSE  [dbo].[F_Retorna_Categoria_Procedimientos](E.IdVenta)END AS Tipos_Procedimientos, E.Segumiento " +
             "FROM EncuestaSatisfaccion E INNER JOIN TiposDocumento T ON E.TipoDocumento=T.IdTipoDocumento INNER JOIN Sedes S ON S.IdSede=E.IdSedeAtencion LEFT JOIN Ventas V ON V.IdVenta=E.IdVenta LEFT JOIN Paciente P ON P.Id_Paciente=V.Id_Paciente " +
             "LEFT JOIN Estratos ET ON ET.IdEstrato=P.IdEstrato LEFT JOIN Localidades L ON L.IdLocalidad=P.IdLocalidad " +
             "LEFT JOIN Opciones O ON O.IdOpc=E.IdEstado  LEFT JOIN TipoProcedimientos TP ON TP.IdTipo=E.TipoProducto " +
             "WHERE CASE WHEN @tipodocumento=0 THEN 0 ELSE E.TipoDocumento END =@tipodocumento AND CASE WHEN @documento='' THEN '' ELSE E.Documento END =@documento " +
             "AND ((E.FechaRegistro >= CAST(@fechaIni AS DATETIME) AND E.FechaRegistro < DATEADD(DAY, 1, CAST(@fechafin AS DATETIME))) OR(@fechaIni = '' AND @fechafin = '')) AND CASE WHEN @sede=0 THEN 0 ELSE E.IdSedeAtencion END =@sede " +
             "AND CASE WHEN @tipoProducto=0 THEN 0 ELSE E.TipoProducto END =@tipoProducto AND CASE WHEN @tipoEvaluacion=0 THEN 0 ELSE E.IdEstado END =@tipoEvaluacion " +
             "ORDER BY E.FechaRegistro DESC";

            return statement;
        }

        private string detailEncuesta()
        {
            string statement = string.Empty;
            {
                statement = @"SELECT d.IdPregunta,Pregunta, CASE WHEN d.IdPregunta IN(5,6,9) THEN STRING_AGG(CONCAT(R.Respuesta, ' ', D.RespuestaAbieta), ', ') ELSE MAX(R.Respuesta) END AS respuesta " +
                    "FROM Encuesta_Detalle D INNER JOIN Encuesta_Preguntas P ON D.IdPregunta = P.IdPregunta LEFT JOIN Encuesta_Tipo_Respuestas R ON D.IdTipoRespuesta = R.IdTipoRespuesta WHERE " +
                    "D.IdEncuesta = @IdEncuesta GROUP BY d.IdPregunta, Pregunta ORDER BY d.IdPregunta";
            }
            return statement;
        }

        private string DetalleExcelEncuesta()
        {
            statement = @"SELECT d.IdPregunta,Pregunta,R.Respuesta,D.RespuestaAbieta " +
                "FROM Encuesta_Detalle D INNER JOIN Encuesta_Preguntas P ON D.IdPregunta = P.IdPregunta LEFT JOIN Encuesta_Tipo_Respuestas R ON D.IdTipoRespuesta = R.IdTipoRespuesta " +
                "WHERE D.IdEncuesta = @IdEncuesta";

            return statement;
        }

        private string ValidadorSolicitud()
        {
            string statement = string.Empty;
            {
                statement = @"SELECT V.IdVenta
				FROM Ventas V INNER JOIN Paciente P ON V.Id_Paciente=P.Id_Paciente " +
                    "WHERE P.Documento=@Documento AND V.IdSolicitudAthenea=@IdSolicitud";
            }
            return statement;
        }

        public string ConsultarInfoServicioSiguiente()
        {
            return InfoServicioSiguiente();
        }


        private string InfoServicioSiguiente()
        {
            statement = @"SELECT TOP 1  IdProcedimiento,IdRegistro FROM Agendamiento_Servicios " +
             "WHERE IdAgendamiento =@IdAgendamiento AND Categoria_Procedimiento IN('EX','CH','PS','V') " +
             "AND IdRegistro > (SELECT MIN(IdRegistro) FROM Agendamiento_Servicios WHERE IdAgendamiento = @IdAgendamiento) " +
             "ORDER BY RegistroTemporal,IdRegistro ASC ";
            return statement;
        }

        public string ActualizarCategoriaMarcacion()
        {
            return ActualizarMarca();
        }
        private string ActualizarMarca()
        {
            statement = @"UPDATE Agendamiento_Servicios SET Categoria_Procedimiento=@categoria 
                        WHERE IdAgendamiento IS NULL AND IdProcedimiento=@IdProcedimiento AND IdUsuario=@IdUsuario ";
            return statement;
        }


        public string ConsultarInfoConsentimientos()
        {
            return DatosPorConsentimiento();
        }


        private string DatosPorConsentimiento()
        {
            statement = @"SELECT Descripcion,
                Beneficios_Alternativas ,
                Riesgos_Implicaciones ,
                Autorizacion,
                Desestimiento,Version,CodigoConsentimiento,CONVERT(VARCHAR, FechaEmision, 23) as FechaEmision,Titulo FROM Consentimientos_Informados A INNER JOIN ConsentimientosInformacion B ON A.IdRegistro=B.IdConsentimiento " +
             "WHERE A.Abreviatura =@proceso";
            return statement;
        }

        public string GetDataSales()
        {
            return BuildGetDataSales();
        }

        private string BuildGetDataSales()
        {
            statement = @"SELECT SL.IdSedeLIS as IdSede, V.IdPlan,PCH.IdPacienteHis, dbo.F_Retorna_TotalPago_Solicitudes(V.IdVenta) as Total,
                CASE WHEN A.CategoriaProcedimiento ='EX' THEN 1
	                 WHEN A.CategoriaProcedimiento ='CH' THEN 2 END AS TipoServicio,CASE WHEN P.IdAcudiente IS NULL THEN 0 ELSE P.IdAcudiente END AS Acudiente,
                V.Urgente,v.IdMedioPago,P.Telefono,P.Direccion
                FROM Ventas V INNER JOIN Paciente P ON V.Id_Paciente=P.Id_Paciente
                LEFT JOIN Paciente_CTS_HIS PCH ON PCH.IdPaciente=P.Id_Paciente
                LEFT JOIN Agendamiento A ON A.IdVenta=V.IdVenta 
                LEFT JOIN Sedes_Lis_Homologacion SL ON V.IdSede=SL.IdSedeCTS
                WHERE V.IdVenta=@IdVenta";
            return statement;
        }

        public string ConsultDataSend()
        {
            return BuildConsultDataSend();
        }

        private string BuildConsultDataSend()
        {
            statement = @"SELECT
                P.Primer_Nombre + ' ' + P.Primer_Apellido AS NombreCompleto,
                T.Abreviatura,
                P.Documento,
                COALESCE(C.Correo, P.Correo) AS Correo, 
                P.Id_Paciente,
                U.Clave
                FROM 
                Ventas V INNER JOIN Paciente P ON V.Id_Paciente=P.Id_Paciente
                INNER JOIN TiposDocumento T ON T.IdTipoDocumento=P.IdTipoDocumento
                LEFT JOIN UsuariosResultados U ON U.Documento=P.Documento AND U.IdTipoDocumento=P.IdTipoDocumento
                LEFT JOIN Paciente C ON C.Id_Paciente = P.IdAcudiente 
                WHERE V.IdVenta=@IdVenta";
            return statement;
        }




        public string ConsultCashRegistersByUser()
        {
            return BuildConsultCashRegistersByUser();
        }


        private string BuildConsultCashRegistersByUser()
        {
            statement = @"SELECT TOP 1 * FROM Apertura_Cierre_Cajas WHERE UsuarioApertura=@user AND Estado=117";
            return statement;
        }


        public string ConsultLastCashRegistersByUser()
        {
            return BuildConsultLastCashRegistersByUser();
        }


        private string BuildConsultLastCashRegistersByUser()
        {
            statement = @"SELECT TOP 1 * FROM Apertura_Cierre_Cajas WHERE UsuarioApertura=@user AND Estado=117 ORDER BY Fecha DESC";
            return statement;
        }

        public string CashRegisterOpening()
        {
            return BuildCashRegisterOpening();
        }

        private string BuildCashRegisterOpening()
        {
            statement = @"INSERT INTO Apertura_Cierre_Cajas (Fecha,UsuarioApertura,MontoBase,IdSede,HoraApertura,Estado)
                VALUES(@fecha,@user,@montobase,@sede,@horaApertura,@estado)
                    SELECT CAST(SCOPE_IDENTITY() as int)";
            return statement;
        }

        public string ConsultBoxArching(int source)
        {
            return BuildConsultBoxArching(source);
        }

        private string BuildConsultBoxArching(int source)
        {
            if (source == 1)
            {
                statement = @"SELECT
                A.IdRegistro as idCaja,
                S.NombreSede,
                A.UsuarioApertura,
                U.NombreCompleto AS Cajero,
                AC.FechaConfirmacion AS FechaBalance,
                  CASE WHEN AC.ConfirmarArqueo =2 OR AC.ConfirmarArqueo IS NULL THEN 'Pendiente' ELSE 'Confirmado' END AS EstadoBalance
                FROM Apertura_Cierre_Cajas A INNER JOIN Sedes S ON A.IdSede=s.IdSede
                INNER JOIN Usuarios U ON A.UsuarioApertura=U.IdUsuario
                LEFT JOIN Apertura_Cierre_Cajas_Detalle AC ON AC.IdCaja=A.IdRegistro
                WHERE 
                 	((A.Fecha >= @fechaInicio  AND A.Fecha < DATEADD(DAY, 1, @fechaFin ))
								OR(@fechaInicio = '' AND @fechaFin = ''))
								AND	CASE WHEN @idSede= 0 THEN 0 ELSE A.IdSede  END=@idSede
								AND CASE WHEN @idUsuario= 0 THEN 0 ELSE A.UsuarioApertura END=@idUsuario
                    AND CASE WHEN @idEstado= 0 THEN 0 ELSE AC.ConfirmarArqueo END=@idEstado;";
            }
            else
            {
                statement = @"SELECT
                            A.IdRegistro as idCaja,
                            u.NombreUsuario+''+S.NombreSede as NombreCaja,
                            S.NombreSede,
                            A.UsuarioApertura,
                            U.NombreCompleto AS Cajero,
                            CONVERT(VARCHAR(10),A.Fecha) +' '+ CONVERT(VARCHAR(8),A.HoraApertura) AS FechaApertura,
                            CONVERT(VARCHAR(10),A.FechaCierre) +' '+ CONVERT(VARCHAR(8),A.HoraCierre)  AS FechaCierre,
                            O.Opcion AS Estado
                            FROM Apertura_Cierre_Cajas A INNER JOIN Sedes S ON A.IdSede=s.IdSede
                            INNER JOIN Usuarios U ON A.UsuarioApertura=U.IdUsuario
                            INNER JOIN Opciones O ON O.IdOpc=A.Estado
                           WHERE 
                 	((A.Fecha >= @fechaInicio  AND A.Fecha < DATEADD(DAY, 1, @fechaFin ))
								OR(@fechaInicio = '' AND @fechaFin = ''))
								AND	CASE WHEN @idSede= 0 THEN 0 ELSE A.IdSede  END=@idSede
								AND CASE WHEN @idUsuario= 0 THEN 0 ELSE A.UsuarioApertura END=@idUsuario;";
            }
            return statement;
        }

        public string DetailBoxArching()
        {
            return BuildDetailBoxArching();
        }

        private string BuildDetailBoxArching()
        {
            statement = @"
                WITH DetalleCaja AS (
                    SELECT 
                        A.IdRegistro AS IdCaja,
                        B.IdMedioEfectivo, 
                        B.MontoEfectivo,
                        B.IdMedioTarjeta, 
                        B.MontoTarjeta,
                        B.IdMedioPse, 
                        B.MontoPse
                    FROM Apertura_Cierre_Cajas A
                    INNER JOIN Apertura_Cierre_Cajas_Detalle B ON A.IdRegistro = B.IdCaja
                    WHERE A.IdRegistro = @IdCaja
                ),
                VentasPorMetodo AS (
                    SELECT 
                        V.IdCaja,
                        V.IdMedioPago AS IdMethod,
                        SUM(VD.PrecioVenta) AS TotalVentas
                    FROM Ventas V
                    INNER JOIN Ventas_Detalle VD ON VD.IdVenta = V.IdVenta
                    WHERE V.IdCaja = @IdCaja AND V.IdEstadoVenta = 2
                    GROUP BY V.IdCaja, V.IdMedioPago
                ),
                MediosPago AS (
                    SELECT 
                        1 AS IdMethod, 'Pse' AS paymentMethodName
                    UNION
                    SELECT 
                        2 AS IdMethod, 'Tarjeta Credito' AS paymentMethodName
                    UNION
                    SELECT 
                        3 AS IdMethod, 'Pago en efectivo' AS paymentMethodName
                )
                SELECT 
                    MP.IdMethod,
                    MP.paymentMethodName,
                    CASE 
                        WHEN MP.IdMethod = 3 THEN DC.MontoEfectivo
                        WHEN MP.IdMethod = 2 THEN DC.MontoTarjeta
                        WHEN MP.IdMethod = 1 THEN DC.MontoPse
                        ELSE 0
                    END AS amountBillingBox,
                    ISNULL(VPM.TotalVentas, 0) AS amountSystem
                FROM MediosPago MP
                LEFT JOIN DetalleCaja DC ON 1 = 1 -- Asociamos siempre DetalleCaja para obtener los montos
                LEFT JOIN VentasPorMetodo VPM ON MP.IdMethod = VPM.IdMethod;";
            return statement;
        }

        public string DetailHistoryBox()
        {
            return BuildDetailHistoryBox();
        }

        private string BuildDetailHistoryBox()
        {
            statement = @"SELECT                         
                        D.MedioPago ,
						CONVERT(VARCHAR,V.FechaVenta,103)+ ' ' + CONVERT(VARCHAR, V.FechaVenta, 108)AS FechaVenta,						
						V.NoFactura,
						V.IdVenta,
						CONCAT_WS(
						' ', 
						RTRIM(COALESCE(P.Primer_Nombre, '')), 
						NULLIF(P.Segundo_Nombre, ''), 
						RTRIM(COALESCE(P.Primer_Apellido, '')), 
						NULLIF(P.Segundo_Apellido, '')
						) AS Nombre_Paciente,
						[dbo].[F_Retorna_TotalPago_Solicitudes](V.IdVenta) AS Valor,
                        D.IdMedioPago,
                        CASE WHEN EXISTS(SELECT 1 FROM Venta_Comprobante VC WHERE VC.IdVenta=V.IdVenta)THEN 'true' ELSE 'false' END AS Comprobante
						FROM 
						VENTAS V INNER JOIN Paciente P ON P.Id_Paciente=V.Id_Paciente
						INNER JOIN MediosPagos D ON D.IdMedioPago=V.IdMedioPago
						INNER JOIN Apertura_Cierre_Cajas A ON A.IdRegistro=V.IdCaja
                        WHERE A.IdRegistro=@IdCaja AND V.IdEstadoVenta=2 
						AND CONVERT(DATE,V.FechaVenta,103) BETWEEN A.Fecha AND A.FechaCierre";
            return statement;
        }


        public string ConsultArching()
        {
            return BuildConsultArching();
        }

        private string BuildConsultArching()
        {
            statement = @"SELECT
                        A.IdRegistro as idCaja,
                        S.NombreSede,
                         CONVERT(VARCHAR(10),A.Fecha) +' '+ CONVERT(VARCHAR(8),A.HoraApertura) AS FechaApertura,
                         CONVERT(VARCHAR(10),A.FechaCierre) +' '+ CONVERT(VARCHAR(8),A.HoraCierre)  AS FechaCierre
                        FROM Apertura_Cierre_Cajas A INNER JOIN Usuarios U ON A.UsuarioApertura=U.IdUsuario
                        INNER JOIN Sedes S ON S.IdSede=A.IdSede
                        INNER JOIN Apertura_Cierre_Cajas_Detalle AC ON A.IdRegistro=AC.IdCaja
                        WHERE A.Estado=118 AND AC.ConfirmarArqueo=2
                        AND CASE WHEN @idUsuario= 0 THEN 0 ELSE A.UsuarioApertura END=@idUsuario";
            return statement;
        }

        public string ConsultarInfofirmasBase64Sol()
        {
            return BuildInfofirmasBase64Sol();
        }

        private string BuildInfofirmasBase64Sol()
        {
            statement = @"SELECT 
            iddocumento,
                       case when P.IdVenta is null then p.IdPaciente else P.IdVenta end AS IdSolicitud,    
             case when P.IdVenta is null then 'Paciente' else 'Solicitud' end AS BanderaCreacion,                
                        P.Cat_Documento,
            FechaFirma
                        FROM Solicitudes_Documentos P
                        WHERE month(FechaFirma)= @mes and year(FechaFirma)=@anio
            AND NOT EXISTS(SELECT 1 FROM FirmaMigradaSolicitud A WHERE A.IdDocumento=P.IdDocumento AND P.Cat_Documento=A.Categoria)            
                        ORDER BY FechaFirma desc";
            return statement;
        }



        public string firmasBase64Sol()
        {
            return BuildfirmasBase64Sol();
        }

        private string BuildfirmasBase64Sol()
        {
            statement = @"SELECT 
                        Documento
                        FROM Solicitudes_Documentos 
                        WHERE iddocumento=@iddocumento";
            return statement;
        }

        public string ConsultPatientPhone()
        {
            return BuildConsultPatientPhone();
        }

        private string BuildConsultPatientPhone()
        {
            statement = @"SELECT 
	            P.Id_Paciente as id, 
	            P.Documento as documento, 
	            P.IdTipoDocumento as tipodocumento,
	            T.Abreviatura as AbreviaturaTipodoc, 
	            P.Primer_Nombre as primerNombre, 
	            P.Segundo_Nombre as segundoNombre, 
	            P.Primer_Apellido as primerApellido, 
	            P.Segundo_Apellido as segundoApellido,P.Fecha_Nacimiento,
	            P.Correo as correo, P.Telefono as telefono, 	
	            IdAcudiente,P.Direccion as direccion,p.Fecha_Nacimiento as fechanacimiento,p.idSexo as Idsexo
		            FROM Paciente P INNER JOIN TiposDocumento T ON P.IdTipoDocumento=T.IdTipoDocumento
			            where P.Activo=1 AND P.Telefono=@telefono
			             ORDER BY P.Id_Paciente DESC";
            return statement;
        }

        #region CONSULTAS PROCESO DE LOGIN

        public string ConsultUserLogin()
        {
            return BuildConsultUserLogin();
        }
        private string BuildConsultUserLogin()
        {
            statement = @"SELECT ACTIVO FROM USUARIOS WHERE NOMBREUSUARIO=@usuario AND CONTRASENA=@contrasena";
            return statement;
        }

        public string ConsultUserData()
        {
            return BuildConsultUserData();
        }
        private string BuildConsultUserData()
        {
            statement = @"SELECT U.IdUsuario, UltimaFechaIngreso, 
                        Usuario_Encriptado,documento,U.IdPerfil,UsuarioAthenea as UsuarioIntegracion, 
                        P.Nombre AS Rol, U.NombreUsuario as Usuario
                        FROM Usuarios U INNER JOIN Perfiles P ON U.idPerfil=P.idPerfil WHERE U.NombreUsuario=@usuario";
            return statement;
        }

        public string ConsultAttentionCenterData()
        {
            return BuildConsultAttentionCenterData();
        }
        private string BuildConsultAttentionCenterData()
        {
            statement = @"SELECT ACTIVO FROM SEDES WHERE IDSEDE=@idSede";
            return statement;
        }
        #endregion

        #region PROCESO SEDES
        public string ConsultSites()
        {
            return BuildConsultSites();
        }

        private string BuildConsultSites()
        {
            statement = @"SELECT IdSede,NombreSede,CASE WHEN Sedes.Activo=1 THEN 'true' ELSE 'false' END AS EstadoNom,Ciudades.Nombre AS Ciudad,
                        Sedes.IdCiudad,Departamentos.IdDepartamento, Sedes.IdAthenea AS CodAthenea, Sedes.CodCredibanco
                        FROM Sedes INNER JOIN Ciudades ON Sedes.IdCiudad=Ciudades.IdCiudad INNER JOIN Departamentos ON Ciudades.IdDepartamentp=Departamentos.IdDepartamento";
            return statement;
        }

        public string ValidateSites()
        {
            return BuildValidateSites();
        }

        private string BuildValidateSites()
        {
            statement = @"SELECT IdSede FROM Sedes WHERE NombreSede = @NombreSede";
            return statement;
        }

        public string CreateOrUpdateSites(string process)
        {
            return BuildCreateOrUpdateSites(process);
        }


        private string BuildCreateOrUpdateSites(string process)
        {
            if (process == "Crear")
            {
                statement = @" INSERT INTO Sedes (IdCiudad, IdAthenea, NombreSede, Activo, CodCredibanco, idUsuario)
                                VALUES (@IdCiudad, @IdAthenea, @NombreSede, @Activo, @CodCredibanco, @IdUsuario);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            }
            else
            {
                statement = @"
                                UPDATE Sedes
                                SET NombreSede = @NombreSede,
                                    IdCiudad = @IdCiudad,
                                    CodCredibanco = @CodCredibanco,
                                    idUsuario = @IdUsuario
                                WHERE IdSede = @IdSede";
            }
            return statement;
        }

        #endregion

        #region PROCESOS PARA PARAMETRIZAR USUARIOS
        public string ConsultUser()
        {
            return BuildConsultUser();
        }

        private string BuildConsultUser()
        {
            statement = @"SELECT
	                    U.IdUsuario,
	                    U.NombreCompleto,U.Documento,U.Correo,U.NombreUsuario,convert(nvarchar,U.FechaNacimiento,101)as FechaNacimiento,
	                    CASE WHEN U.Activo!=1 THEN 2 ELSE U.Activo END AS Estado,
	                    P.Nombre AS Perfil,U.IdPerfil,
	                    CASE WHEN U.Activo=1 THEN 'true' ELSE 'false' END AS EstadoNom, U.UsuarioAthenea,Usuario_Encriptado,U.Firma,U.Idtipodocumento
	                    FROM USUARIOS U INNER JOIN PERFILES P ON U.IdPerfil=P.IdPerfil
	                    where u.CodigoProyecto=1 OR u.CodigoProyecto IS NULL";
            return statement;
        }

        public string ConsultUserId()
        {
            return BuildConsultUserId();
        }
        private string BuildConsultUserId()
        {
            statement = @"	SELECT
	                U.IdUsuario,
	                U.NombreCompleto,U.Documento,U.Correo,U.NombreUsuario,convert(nvarchar,U.FechaNacimiento,101)as FechaNacimiento,	
	                CASE WHEN U.Activo=1 THEN 1 ELSE 2 END AS Estado,
	                P.Nombre AS Perfil, u.FechaCreacion,u.UltimaFechaIngreso,U.UsuarioAthenea,
	                (select STUFF(
		                (SELECT CAST(',' AS varchar(MAX)) + S.NombreSede
		                FROM Sedes S INNER JOIN Sedes_x_Usuario SU ON S.IdSede=SU.IdSede WHERE U.IdUsuario=SU.IdUsuario
		                ORDER BY S.IdSede
		                FOR XML PATH('')
		                ), 1, 1, '')) as Sedes,U.Firma
	                FROM USUARIOS U INNER JOIN PERFILES P ON U.IdPerfil=P.IdPerfil
	                WHERE U.IdUsuario=@id";

            return statement;
        }
        #endregion

        #region PROCESOS PARA CONTROLADOR GENERICO
        public string ConsultDepartments()
        {
            return BuildConsultDepartments();
        }

        private string BuildConsultDepartments()
        {
            statement = @"SELECT
	            IdDepartamento,
	            Nombre
	            FROM
	            Departamentos
	            WHERE Activo=1";
            return statement;
        }

        public string ConsultStateSales()
        {
            return BuildConsultStateSales();
        }

        private string BuildConsultStateSales()
        {
            statement = @"SELECT DISTINCT	
			CASE WHEN V.IdEstadoVenta=1 AND IdMedioPago= '' THEN 1
				 WHEN V.IdEstadoVenta=2  THEN 2
				 WHEN V.IdEstadoVenta=3 THEN 3 
				 WHEN V.IdEstadoVenta=4 THEN 4 
				 WHEN V.IdEstadoVenta=5 THEN 5 
				 WHEN V.IdEstadoVenta=1 AND IdMedioPago IS NOT NULL THEN 6
				 
				 END AS Id,
			CASE WHEN V.IdEstadoVenta=1 AND IdMedioPago= ''  THEN 'Creada'
				 WHEN V.IdEstadoVenta=1 AND IdMedioPago IS NOT NULL THEN 'Por facturar'		
				 WHEN V.IdEstadoVenta=2  THEN 'Facturada'
				 WHEN V.IdEstadoVenta=3 THEN 'Anulada'
				 WHEN V.IdEstadoVenta=4 THEN 'Pendiente' 
				 WHEN V.IdEstadoVenta=5 THEN 'Confirmada' END AS Nombre

		FROM Ventas V INNER JOIN EstadoVentas EV ON V.IdEstadoVenta=EV.IdEstado	";
            return statement;
        }

        public string ConsultOptions()
        {
            return BuildConsultOptions();
        }

        private string BuildConsultOptions()
        {
            statement = @"SELECT 
			O.IdOpc,O.Opcion,
			O.Icono
			FROM Opciones O INNER JOIN Opciones_Permisos OP ON O.IdOpc=OP.IdOpcion
			WHERE 
			IdCatOpc=@id AND OP.IdPerfil=@perfil AND OP.Acceso=1";
            return statement;
        }

        public string ConsultAdviser()
        {
            return BuildConsultAdviser();
        }

        private string BuildConsultAdviser()
        {
            statement = @"	SELECT IdUsuario,NombreCompleto
			FROM Usuarios
			WHERE IdPerfil =@perfil and Activo=1
			ORDER BY NombreCompleto ASC	";
            return statement;
        }

        public string ConsultStateRequest()
        {
            return BuildConsultStateRequest();
        }

        private string BuildConsultStateRequest()
        {
            statement = @"DECLARE @TabTemporal AS TABLE(IdEstado_Solicitud INT, Estado_Solicitud VARCHAR(200))
			INSERT INTO @TabTemporal
			SELECT DISTINCT			
			CASE WHEN V.IdEstadoVenta=1 AND IdMedioPago= '' OR IdMedioPago IS NULL THEN 1
				 WHEN V.IdEstadoVenta=1 AND IdMedioPago IS NOT NULL THEN 5
				 WHEN V.IdEstadoVenta=2  THEN 2
				 WHEN V.IdEstadoVenta=3 THEN 3 END AS IdEstado_Solicitud,
		
		CASE WHEN V.IdEstadoVenta=1 AND IdMedioPago= ''  OR IdMedioPago IS NULL THEN 'Creada'
				 WHEN V.IdEstadoVenta=1 AND IdMedioPago IS NOT NULL THEN 'Por facturar'		
				 WHEN V.IdEstadoVenta=2  THEN 'Facturada'
				 WHEN V.IdEstadoVenta=3 THEN 'Anulada'END AS Estado_Solicitud	

		FROM Ventas V 	INNER JOIN EstadoVentas EV ON V.IdEstadoVenta=EV.IdEstado
		WHERE V.IdEstadoVenta NOT IN(4,5) 
		SELECT*FROM @TabTemporal ORDER BY IdEstado_Solicitud";
            return statement;
        }


        public string CreateContentsProcedures(string categoria)
        {
            return BuildCreateContentsProcedures(categoria);
        }

        private string BuildCreateContentsProcedures(string categoria)
        {
            switch (categoria.ToUpper())
            {
                case "CH":
                    statement = "UPDATE Chequeos SET Contenido_Html = @ContenidoHTML WHERE IdChequeo = @IdProcedimiento";
                    break;
                case "EX":
                    statement = "UPDATE Examenes SET Contenido_Html = @ContenidoHTML WHERE IdExamen = @IdProcedimiento";
                    break;
                case "PR":
                    statement = "UPDATE Programas SET Contenido_Html = @ContenidoHTML WHERE IdPrograma = @IdProcedimiento";
                    break;
                case "CM":
                    statement = "UPDATE ConsultasMedicas SET Contenido_Html = @ContenidoHTML WHERE IdConsulta = @IdProcedimiento";
                    break;
                case "V":
                    statement = "UPDATE Vacunas SET Contenido_Html = @ContenidoHTML WHERE IdVacuna = @IdProcedimiento";
                    break;
                default:
                    throw new ArgumentException("Categoría inválida");
            }
            return statement;
        }
        #endregion
    }
}

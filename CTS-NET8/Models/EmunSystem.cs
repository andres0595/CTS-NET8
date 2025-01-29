using System.ComponentModel;
using System.Reflection;

namespace CTS_NET8.Models
{
    public enum EmunSystem
    {
        [Description("1")]
        MedioPagoPse,
        [Description("2")]
        MedioPagoTarjetaCredito,
        [Description("3")]
        MedioPagoEfectivo,
        [Description("25")]
        FiltroOpcionesFacturacion,
        [Description("1")]
        EstadoFiltroOpcionesFacturacion,
        [Description("95")]
        FacturacionAthenea,
        [Description("99")]
        FacturacionLiveLis,
        [Description("VITALEA")]
        banderaBlobVitalea,
        [Description("VIOLETA")]
        banderaBlobVioleta,
        [Description("Development")]
        ambienteDesarrollo,
        [Description("Test")]
        ambientePruebas,
        [Description("Production")]
        ambienteProduccion,
        [Description("Usuario o contraseña no válidos")]
        UsuarioNoValido,
        [Description("El usuario se encuentra inactivo. Por favor comunícate con el administrador del sistema.")]
        UsuarioInactivo,
        [Description("La sede seleccionada se encuentra inactiva. Por favor comunícate con el administrador del sistema.")]
        SedeInactiva,
        [Description("El usuario no cuenta con módulos asignados. Por favor comunícate con el administrador del sistema.")]
        UsuarioSinModulos,
        [Description("la autenticación del usuario fue exitosa")]
        AutenticacionExitosa,
        [Description("Acceso concedido, debes realizar el cambio de contraseña por primera vez")]
        AutenticacionPrimeraVez,
    }
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}

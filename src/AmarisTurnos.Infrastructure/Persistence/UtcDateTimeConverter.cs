using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AmarisTurnos.Infrastructure.Persistence
{
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter() : base(
        v => v, // al guardar: tal cual (SQL Server no distingue Kind de todas formas)
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)) // al leer: marca explícitamente como UTC
        {
        }
    }
}

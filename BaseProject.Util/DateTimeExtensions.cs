using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Util
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Converte a hora de um DateTime para o horário padrão de Brasília
		/// </summary>
		/// <returns>Retorna o DateTime com o horário padrão de Brasília</returns>
		public static DateTime ToBrasiliaTime(this DateTime dateTime)
		{
			dateTime = dateTime.ToUniversalTime();
			var horaBrasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime, horaBrasilia);
		}

        /// <summary>
        /// Calcula a diferença de meses entre duas datas
        /// </summary>
        /// <returns>Retorna um inteiro da diferença</returns>
        public static int MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }
    }
}

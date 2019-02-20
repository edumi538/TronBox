using System;

namespace TronBox.Infra.Data.Utilitarios
{
    public class UtilitarioDatas
    {
        /// <summary>
        /// Lógica para cálculo do dia e mês de vencimento para a vigência.
        /// </summary>
        class Vigencia
        {
            private int[] mesesPeriodo = new[] { 1, 2, 3, 6, 12 };

            /// <summary>
            /// Quantidade de dias que devem ser somados á data inicial do mês em que vence a obrigação.
            /// </summary>
            public int DiasSomar { get; set; }
            /// <summary>
            /// Quantidade de meses a somar ao mês de cálculo.
            /// </summary>
            public int MesesSomar { get; set; }

        }

        public static DateTime AdicioneDiasUteis(DateTime data, int quantidadeDias)
        {
            int i = 0;
            while (i < quantidadeDias)
            {
                switch (data.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        data = data.AddDays(2);
                        break;
                    case DayOfWeek.Sunday:
                        data = data.AddDays(1);
                        break;
                    default:
                        data = data.AddDays(1);
                        i++;
                        break;
                }
            }
            return data;
        }
    }
}

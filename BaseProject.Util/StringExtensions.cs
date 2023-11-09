using System.Globalization;
using System.Text;

namespace BaseProject.Util
{
	public static class StringExtensions
    {
        public static string[] Abreviatte(this string nome)
        {
            var nomesAbreviados = new List<string>();

            if (!nome.Contains(' ')) return nomesAbreviados.ToArray();

            nome = nome.Replace(".", "").RemoveDiacritics().ToUpper().Trim();

            while (nome.Contains("  ")) nome = nome.Replace("  ", " ");

            //Insere o nome completo normal
            if (!nomesAbreviados.Contains(nome)) nomesAbreviados.Add(nome);

            var preposicoes = new string[] { " DE ", " DA ", " DO ", " DAS ", " DOS " };

            foreach (var p in preposicoes) nome = nome.Replace(p, " ");

            //Insere o nome completo sem as preposições
            if (!nomesAbreviados.Contains(nome)) nomesAbreviados.Add(nome);

            var nomes = nome.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            int indexPrimeiroNome = 0;
            int indexUltimoNome = nomes.Length - 1;

            //Insere o nome completo com nomes do meio abreviados e variações de cortes no ultimo nome se necessário
            if (nomes.Length > 2)
            {
                var nomeSemUltimo = nomes[indexPrimeiroNome] + " ";

                for (int i = indexPrimeiroNome + 1; i < indexUltimoNome; i++) nomeSemUltimo += nomes[i].First() + " ";

                if (nomes.Length > 4)
                {
                    var ultimoNome = string.Empty;
                    foreach (var c in nomes[indexUltimoNome])
                    {
                        nome = nomeSemUltimo + (ultimoNome += c);
                        if (!nomesAbreviados.Contains(nome)) nomesAbreviados.Add(nome);
                    }
                }
                else
                {
                    nome = nomeSemUltimo + nomes[indexUltimoNome];
                    if (!nomesAbreviados.Contains(nome)) nomesAbreviados.Add(nome);
                }
            }

            //Insere o nome completo com variações de cortes no ultimo nome se necessário
            if (nomes.Length > 4)
            {
                var nomeSemUltimo = nomes[indexPrimeiroNome] + " ";

                for (int i = indexPrimeiroNome + 1; i < indexUltimoNome; i++) nomeSemUltimo += nomes[i] + " ";

                var ultimoNome = string.Empty;
                foreach (var c in nomes[indexUltimoNome])
                {
                    nome = nomeSemUltimo + (ultimoNome += c);
                    if (!nomesAbreviados.Contains(nome)) nomesAbreviados.Add(nome);
                }
            }

            return nomesAbreviados.DistinctBy(x => x).ToArray();
        }

        public static string GetRacaValue(string value)
        {
            switch (value)
            {
                case "Branco":
                    return "Branca";
                case "Preto":
                    return "Preta";
                case "Pardo":
                    return "Parda";
                case "Indígena":
                    return "Indigena";
                case "Amarelo":
                    return "Amarela";
                case "Não desejo informar":
                    return "Indisponível";
                default:
                    return value;

            }
        }

        public static string ToUTF8(this string text)
        {
            byte[] bytes = new byte[text.Length];

            for (int i = 0; i < text.Length; ++i)
            {
                bytes[i] = (byte)text[i];
            }

            text = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            return text;
        }

        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&(){}[]/";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomNumber(int length)
        {
            const string chars = "1234567890";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomPassword(int length)
        {
            if(length < 8) length = 8;
            if (length > 16) length = 16;

            int remainder = length % 4;
            int count = length / 4;

            const string specials = "$*&@#!?%";
            const string charsMin = "abcdefghijklmnopqrstuvwxyz";
            const string charsMax = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";            

            char[] password = Array.Empty<char>();

            password = password.Concat(Enumerable.Repeat(specials, count).Select(s => s[random.Next(s.Length)]).ToArray()).ToArray();
            password = password.Concat(Enumerable.Repeat(charsMin, count).Select(s => s[random.Next(s.Length)]).ToArray()).ToArray();
            password = password.Concat(Enumerable.Repeat(charsMax, count).Select(s => s[random.Next(s.Length)]).ToArray()).ToArray();
            password = password.Concat(Enumerable.Repeat(numbers, remainder > 0 ? (count + remainder) : count).Select(s => s[random.Next(s.Length)]).ToArray()).ToArray();

            Randomize(password);

            return new string(password);
        }

        public static void Randomize<T>(T[] items)
        {
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = random.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }

        public static string RandomCPF(bool masked = true)
        {
            const string numbers = "0123456789";

            if (masked)
            {
                return new string(Enumerable.Repeat(numbers, 3).Select(s => s[random.Next(s.Length)]).ToArray()) + "."
                 + new string(Enumerable.Repeat(numbers, 3).Select(s => s[random.Next(s.Length)]).ToArray()) + "."
                 + new string(Enumerable.Repeat(numbers, 3).Select(s => s[random.Next(s.Length)]).ToArray()) + "-"
                 + new string(Enumerable.Repeat(numbers, 2).Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return new string(Enumerable.Repeat(numbers, 11).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string MaskCNPJ(this string text)
        {
            string retorno = text ?? string.Empty;
            if (retorno.Any(c => !char.IsDigit(c)) || retorno.Length != 14)
                return text;

            return retorno.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
        }

        public static string MaskCPF(this string text)
        {
            string retorno = text ?? string.Empty;
            if (retorno.Any(c => !char.IsDigit(c)) || retorno.Length != 11)
                return text;

            return retorno.Insert(3, ".").Insert(7, ".").Insert(11, "-");
        }

        public static string MaskTelefone(this string text)
        {
            string retorno = text ?? string.Empty;

            if (retorno.Any(t => !char.IsDigit(t)) || (retorno.Length != 10 && retorno.Length != 11))
                return text;

            if(retorno.Length == 10) return retorno.Insert(0, "(").Insert(3, ") ").Insert(9, "-");

            return retorno.Insert(0, "(").Insert(3, ") ").Insert(10, "-");
        }

        public static string MaskCEP(this string text)
        {
            string retorno = text ?? string.Empty;
            if (retorno.Any(t => !char.IsDigit(t)) || retorno.Length != 8)
                return text;

            return retorno.Insert(5, "-");
        }

        public static string Unmask(this string text)
        {
            var builder = new StringBuilder();
            var caracteres = text.ToArray();

            for (int i = 0; i < caracteres.Length; i++)
            {
                if (char.IsNumber(caracteres[i]))
                {
                    builder.Append(caracteres[i]);
                }
            }

            return builder.ToString();
        }

        public static string ConverterEstados(this string text)
        {

            switch (text.ToUpper())
            {
                /* UFs */
                case "AC": text = "Acre"; break;
                case "AL": text = "Alagoas"; break;
                case "AM": text = "Amazonas"; break;
                case "AP": text = "Amapá"; break;
                case "BA": text = "Bahia"; break;
                case "CE": text = "Ceará"; break;
                case "DF": text = "Distrito Federal"; break;
                case "ES": text = "Espírito Santo"; break;
                case "GO": text = "Goiás"; break;
                case "MA": text = "Maranhão"; break;
                case "MG": text = "Minas Gerais"; break;
                case "MS": text = "Mato Grosso do Sul"; break;
                case "MT": text = "Mato Grosso"; break;
                case "PA": text = "Pará"; break;
                case "PB": text = "Paraíba"; break;
                case "PE": text = "Pernambuco"; break;
                case "PI": text = "Piauí"; break;
                case "PR": text = "Paraná"; break;
                case "RJ": text = "Rio de Janeiro"; break;
                case "RN": text = "Rio Grande do Norte"; break;
                case "RO": text = "Rondônia"; break;
                case "RR": text = "Roraima"; break;
                case "RS": text = "Rio Grande do Sul"; break;
                case "SC": text = "Santa Catarina"; break;
                case "SE": text = "Sergipe"; break;
                case "SP": text = "São Paulo"; break;
                case "TO": text = "Tocantíns"; break;

                /* Estados */
                case "ACRE": text = "AC"; break;
                case "ALAGOAS": text = "AL"; break;
                case "AMAZONAS": text = "AM"; break;
                case "AMAPÁ": text = "AP"; break;
                case "BAHIA": text = "BA"; break;
                case "CEARÁ": text = "CE"; break;
                case "DISTRITO FEDERAL": text = "DF"; break;
                case "ESPÍRITO SANTO": text = "ES"; break;
                case "GOIÁS": text = "GO"; break;
                case "MARANHÃO": text = "MA"; break;
                case "MINAS GERAIS": text = "MG"; break;
                case "MATO GROSSO DO SUL": text = "MS"; break;
                case "MATO GROSSO": text = "MT"; break;
                case "PARÁ": text = "PA"; break;
                case "PARAÍBA": text = "PB"; break;
                case "PERNAMBUCO": text = "PE"; break;
                case "PIAUÍ": text = "PI"; break;
                case "PARANÁ": text = "PR"; break;
                case "RIO DE JANEIRO": text = "RJ"; break;
                case "RIO GRANDE DO NORTE": text = "RN"; break;
                case "RONDÔNIA": text = "RO"; break;
                case "RORAIMA": text = "RR"; break;
                case "RIO GRANDE DO SUL": text = "RS"; break;
                case "SANTA CATARINA": text = "SC"; break;
                case "SERGIPE": text = "SE"; break;
                case "SÃO PAULO": text = "SP"; break;
                case "TOCANTÍNS": text = "TO"; break;
                default: text = "N/A";  break;
            }

            return text;
        }
    }
}

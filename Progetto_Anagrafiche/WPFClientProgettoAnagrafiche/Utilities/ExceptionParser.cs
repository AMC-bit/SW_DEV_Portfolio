using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WPFClientProgettoAnagrafiche.Utilities
{
    public static class ExceptionParser
    {
        public static string ParseApiException(string exceptionMessage)
        {
            try
            {
                // split the message at "{" , trim spaces
                string mainError = exceptionMessage.Split("{")[0].Trim();

                // where is { ?
                int jsonStart = exceptionMessage.IndexOf("{");

                // pick everything that comes after that position
                string jsonResponse = exceptionMessage.Substring(jsonStart);

                // convert to jsondocument (similar pathing to xml)
                JsonDocument doc = JsonDocument.Parse(jsonResponse);

                // grab root (same as xml)
                JsonElement root = doc.RootElement;

                // get the property "errors"
                JsonElement errorsElement = root.GetProperty("errors");

                // structure output like this
                string output = "API RESPONSE:\n" + mainError + "\n\nVALIDATION ERRORS:\n";

                // loop loop through errorselement
                foreach (JsonElement error in errorsElement.EnumerateArray())
                {
                    // add each error message to the output string with a new line
                    output += error.GetString() + "\n";
                }

                // return the final output string
                return output;
            }
            catch
            {
                // If JSON parsing fails, return the original message
                return exceptionMessage;
            }
        }
    }
}

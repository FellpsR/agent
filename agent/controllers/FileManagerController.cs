using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.AccessControl;
using FileManager.Model;


[Route("link")]
[ApiController]
public class FileManagerController : ControllerBase
{
    [HttpPost]
    public IActionResult CheckFile([FromBody] FileManagerModel request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url))
            {
                return BadRequest(new { message = "Caminho informado em branco ou nulo!" });
            }

            string path = request.Url;

            if (System.IO.File.Exists(path) || Directory.Exists(path))
            {
                try
                {   
                    //Tenta abrir o arquivo ou diretório.
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });

                    // Verifica as permissões do diretório ou arquivo.
                    var accessControl = System.IO.File.Exists(path) ? (FileSystemSecurity)new FileSecurity(path, AccessControlSections.Access) : new DirectorySecurity(path, AccessControlSections.Access);
                    var rules = accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                    Console.WriteLine("passou1");

                    // Verifica se o usuário atual tem permissão para acessar o diretório ou arquivo.
                    string currentUser = "";
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    }
                    var hasPermission = false;
                    Console.WriteLine("passou2");
                    foreach (FileSystemAccessRule rule in rules)
                    {
                        if (!(rule.IdentityReference.Value.Equals(currentUser, StringComparison.CurrentCultureIgnoreCase) || rule.AccessControlType != AccessControlType.Allow))
                        {
                            hasPermission = true;
                            Console.WriteLine("passou3");
                            break;
                        }
                    

                        if (!hasPermission)
                        {
                            Console.WriteLine("passou4");
                            return Forbid();
                        }
                    }

                    // Se o caminho é um arquivo/diretório válido, return msg ao console.
                    Console.WriteLine();
                    return Ok(new { message = "path aberto com sucesso." });
                }
                catch (SystemException ex)
                {
                    Console.WriteLine("passou5");
                    var msg = "Erro ao abrir arquivo. " + ex.Message;
                    return BadRequest(new { message = msg });
                }
            }
            else
            {
                // Se o caminho não existe, retorna um não encontrado.
                Console.WriteLine("Path não encontrado.");
                return StatusCode(404, "Path não encontrado.");
            }
        }
        catch (Exception ex)
        {
            // Lidar com exceções, se necessário.
            Console.WriteLine("Erro Interno de Servidor");
            var msg = "Erro interno de Servidor." + ex.Message;
            return StatusCode(500, new { message = msg });
        }
    }
}

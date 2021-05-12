using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MvcCoreAWSDynamoDBS3.Helpers;
using MvcCoreAWSDynamoDBS3.Models;
using MvcCoreAWSDynamoDBS3.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAWSDynamoDBS3.Controllers
{
    public class UsuariosController : Controller
    {
        private ServiceAWSDynamoDB ServiceAWSDynamo;
        private ServiceAWSS3 ServiceAWSS3;
        private UploadHelper uploadhelper;
        private String urlFoto;


        public UsuariosController (ServiceAWSDynamoDB serviceDynamo, ServiceAWSS3 serviceAWSS3, UploadHelper uploadhelper, IConfiguration configuration)
        {
            this.ServiceAWSDynamo = serviceDynamo;
            this.ServiceAWSS3 = serviceAWSS3;
            this.uploadhelper = uploadhelper;
            this.urlFoto = "https://" + configuration["AWSS3:BucketName"] + ".s3.amazonaws.com/";
        }

        public async Task<IActionResult> Index()
        {
            List<Usuario> usuarios = await this.ServiceAWSDynamo.GetUsuariosAsync();
            if (usuarios.Count <= 0)
            {
                ViewData["MENSAJE"] = "No se han encontrado usuarios";
                return View();
            }
            return View(usuarios);
        }


        public async Task<IActionResult> Details(int id)
        {
            return View(await this.ServiceAWSDynamo.FindUsuarioAsync(id));
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Usuario user, 
            String titulo, String imagen, IFormFile file1,
            String titulo2, String imagen2, IFormFile file2,
            String titulo3, String imagen3, IFormFile file3)
        {
            if(titulo != null)
            {
                List<Foto> fotos = new List<Foto>();

                Foto foto = new Foto();
                foto.Titulo = titulo;
                foto.Imagen = await this.SubirFoto(file1, ToolKit.Normalize(imagen));
                
                fotos.Add(foto);
                
                

                if (titulo2 != null)
                {
                    Foto foto2 = new Foto();
                    foto2.Titulo = titulo2;
                    foto2.Imagen = await this.SubirFoto(file2, ToolKit.Normalize(imagen2));

                    fotos.Add(foto2);
                }
                if (titulo3 != null)
                {
                    Foto foto3 = new Foto();
                    foto3.Titulo = titulo3;
                    foto3.Imagen = await this.SubirFoto(file3, ToolKit.Normalize(imagen3));

                    fotos.Add(foto3);
                }
                user.Fotos = fotos;
            }
            await this.ServiceAWSDynamo.CreateUsuarioAsync(user);
            return RedirectToAction("Index");
        }
        private async Task<String> SubirFoto(IFormFile file, String imagen)
        {
            imagen = imagen + ".jpg";
            String nombrefoto = this.urlFoto + imagen;
            //PRIMERO A LOCAL CON EL HELPER
            String path =
                await this.uploadhelper.UploadFileAsync(file, Folders.Images, imagen);
            //SUBIMOS EL FICHERO LOCAL A AWS
            using (FileStream stream = new FileStream(path
                , FileMode.Open, FileAccess.Read))
            {
                bool respuesta =
                    await this.ServiceAWSS3.UploadFileAsync(stream, imagen);
                ViewData["MENSAJE"] = "Archivo en AWS Bucket: " + respuesta;
            };
            return nombrefoto;
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await this.ServiceAWSDynamo.FindUsuarioAsync(id));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Usuario user)
        {
            await this.ServiceAWSDynamo.UpdateUsuarioAsync(user.IdUsuario, user.Nombre, user.Descripcion, user.FechaAlta);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Usuario usuario = await this.ServiceAWSDynamo.FindUsuarioAsync(id);
            await this.ServiceAWSDynamo.DeleteUsuarioAsync(id);
            foreach(Foto foto in usuario.Fotos)
            {
                String nombrefoto = foto.Imagen;
                nombrefoto = nombrefoto.Substring(nombrefoto.LastIndexOf("/")+1);
                await this.ServiceAWSS3.DeleteFileAsync(nombrefoto);
            }
            return RedirectToAction("Index");
        }
    }
}

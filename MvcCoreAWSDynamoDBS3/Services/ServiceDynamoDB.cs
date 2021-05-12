using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MvcCoreAWSDynamoDBS3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAWSDynamoDBS3.Services
{
    public class ServiceAWSDynamoDB
    {
        private DynamoDBContext context;

        public ServiceAWSDynamoDB()
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            this.context = new DynamoDBContext(client);
        }

        public async Task CreateUsuarioAsync(Usuario user)
        {
            await this.context.SaveAsync<Usuario>(user);
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            var table = this.context.GetTargetTable<Usuario>();
            var scanOptions = new ScanOperationConfig();
            var result = table.Scan(scanOptions);

            List<Document> data = await result.GetNextSetAsync();
            IEnumerable<Usuario> usuarios = this.context.FromDocuments<Usuario>(data);

            return usuarios.ToList();
        }

        public async Task<Usuario> FindUsuarioAsync(int idUsuario)
        {
            return await this.context.LoadAsync<Usuario>(idUsuario);
        }

        public async Task DeleteUsuarioAsync(int idUsuario)
        {
            await this.context.DeleteAsync<Usuario>(idUsuario);
        }

        public async Task UpdateUsuarioAsync(int IdUsuario, String nombre, String desc,
            String fecha)
        {
            Usuario user = await this.FindUsuarioAsync(IdUsuario);
            user.Nombre = nombre;
            user.Descripcion = desc;
            user.FechaAlta = fecha;

            await this.context.SaveAsync<Usuario>(user);
        }

    }
}

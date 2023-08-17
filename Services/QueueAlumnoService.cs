using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KalumManagement.Dtos;
using RabbitMQ.Client;

namespace KalumManagement.Services
{
    public class QueueAlumnoService : IQueueAlumnoService
    {
        public readonly ILogger<QueueAlumnoService> Logger;

        public QueueAlumnoService(ILogger<QueueAlumnoService> _logger)
        {
            this.Logger = _logger;
        }
        public async Task<bool> CrearSolicitudAlumnoAsync(AlumnoCreateDTO alumno)
        {
            bool response = false;
            ConnectionFactory connectionFactory = new ConnectionFactory();
            IConnection conexion = null;
            IModel channel = null;
            connectionFactory.HostName = "localhost";
            connectionFactory.VirtualHost = "/";
            connectionFactory.Port = 5672;
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";

            try
            {
                conexion = connectionFactory.CreateConnection();
                channel = conexion.CreateModel();
                channel.BasicPublish("kalum.exchange.aspirante", "alumno", null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(alumno)));
                response = true;
                await Task.Delay(100);
            }
            catch (Exception e)
            {
                
                Logger.LogError(e.Message);
            }
            finally
            {
                channel.Close();
                conexion.Close();
            }

            return response;
        }
    }
}
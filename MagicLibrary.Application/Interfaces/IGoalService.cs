using MagicLibrary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLibrary.Application.Interfaces
{
    public interface IGoalService
    {
        Goal ObtenerMetaOCrearPorDefecto(int idUsuario, int anio);
        int CalcularDiasRestantesAnio(int diasPorSemana=7);
        List<Goal> ObtenerTodos();
        Goal? ObtenerPorId(int id);
        void Agregar(Goal goal);
        void Actualizar(Goal goal);
        Goal? ObtenerMetaActual(int idUsuario, int anio);
        void ConfirmarLibroAgregado(Goal goal);
        //para codesmell
        int CalcularTotalPaginasPendientes(Goal metaActual);
        void CompletarLibroEnMeta(int idUsuario, int anio, string tituloLibro);
        void ConfirmarLecturaDiaria(int idUsuario, int anio);
    }
}

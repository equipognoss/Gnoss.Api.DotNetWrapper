using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnoss.Apiwrapper.ApiModel
{
    public partial class Rol
    {
        public Rol()
        {
        }
        public Guid RolID { get; set; }
        public string Nombre { get; set; }
        public List<PermisoModel> ListaPermisos { get; set; }
    }

    public partial class PermisoModel
    {
        public string Nombre { get; set; }

        public string Seccion { get; set; }

        public bool Concedido { get; set; }
    }

    #region parametros para el add
    public class ParamsRoleCommunity
    {
        /// <summary>
        /// Rol identifier
        /// </summary>
        public Guid? rol_id { get; set; }
        /// <summary>
        /// Short name of the community to add the role
        /// </summary>
        public string community_short_name { get; set; }

        /// <summary>
        /// Community identifier
        /// </summary>
        public Guid? community_id { get; set; }

        /// <summary>
        /// Rol name
        /// </summary>
        public string pNombre { get; set; }

        /// <summary>
        /// Community description
        /// </summary>
        public string pDescripcion { get; set; }

        /// <summary>
        /// Community scope (community/ecosystem)
        /// </summary>
        public AmbitoRol pAmbito { get; set; }

        /// <summary>
        /// Permissions with PermisosDTO
        /// </summary>
        public PermisosDTO pPermisos { get; set; }

        /// <summary>
        /// Resources permissions with PermisosRecursosDTO
        /// </summary>
        public PermisosRecursosDTO pPermisosRecursos { get; set; }

        /// <summary>
        /// Ecosystem permissions with PermisosEcosistemaDTO
        /// </summary>
        public PermisosEcosistemaDTO pPermisosEcosistema { get; set; }

        /// <summary>
        /// Content permissions with PermisosContenidosDTO
        /// </summary>
        public PermisosContenidosDTO pPermisosContenidos { get; set; }

        /// <summary>
        /// Semantic Resources permissions
        /// </summary>
        public Dictionary<Guid, DiccionarioDePermisos> pPermisosRecursosSemanticos { get; set; }


    }
    #endregion


    public enum AmbitoRol
    {
        Comunidad,
        Ecosistema
    }


    public partial class DiccionarioDePermisos
    {
        public bool CrearRecursoSemantico { get; set; }
        public bool EditarRecursoSemantico { get; set; }
        public bool EliminarRecursoSemantico { get; set; }
        public bool RestaurarVersionRecursoSemantico { get; set; }
        public bool EliminarVersionRecursoSemantico { get; set; }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class SectionAttribute : Attribute
    {
        public string Section { get; }

        public SectionAttribute(string section)
        {
            Section = section;
        }
    }


    #region PERMISOS DE LA COMUNIDAD
    public class PermisosDTO
    {
        // COMUNIDAD
        [Description("DESCPERMISOINFOGENERAL")]
        [Section("COMUNIDAD")]
        public bool GestionarInformacionGeneral { get; set; }

        [Description("DESCPERMISOFLUJOS")]
        [Section("COMUNIDAD")]
        public bool GestionarFlujos { get; set; }

        [Description("DESCPERMISOINTERACCIONESSOCIALES")]
        [Section("COMUNIDAD")]
        public bool GestionarInteraccionesSociales { get; set; }

        [Description("DESCPERMISOMIEMBROS")]
        [Section("COMUNIDAD")]
        public bool GestionarMiembros { get; set; }

        [Description("DESCPERMISOSOLICITUDESGRUPO")]
        [Section("COMUNIDAD")]
        public bool GestionarSolicitudesDeAccesoAGrupo { get; set; }

        [Description("DESCPERMISONIVELESCERTIFICACION")]
        [Section("COMUNIDAD")]
        public bool GestionarNivelesDeCertificacion { get; set; }

        [Description("DESCPERMISOROLES")]
        [Section("COMUNIDAD")]
        public bool GestionarRolesYPermisos { get; set; }

        // ESTRUCTURA
        [Description("DESCPERMISOPESOSAUTOCOMPLETAR")]
        [Section("ESTRUCTURA")]
        public bool GestionarPesosAutocompletado { get; set; }

        [Description("DESCPERMISOREDIRECCIONES")]
        [Section("ESTRUCTURA")]
        public bool GestionarRedirecciones { get; set; }

        // CONFIGURACIÓN
        [Description("DESCPERMISOOAUTH")]
        [Section("CONFIGURACION")]
        public bool DescargarConfiguracionOAuth { get; set; }

        [Description("DESCPERMISOCOOKIES")]
        [Section("CONFIGURACION")]
        public bool GestionarCookies { get; set; }

        [Description("DESCPERMISOFTP")]
        [Section("CONFIGURACION")]
        public bool AccederAlFTP { get; set; }

        [Description("DESCPERMISOTRADUCCIONES")]
        [Section("CONFIGURACION")]
        public bool GestionarTraducciones { get; set; }

        [Description("DESCPERMISODATOSEXTRA")]
        [Section("CONFIGURACION")]
        public bool GestionarDatosExtraRegistro { get; set; }

        [Description("DESCPERMISOTRAZAS")]
        [Section("CONFIGURACION")]
        public bool GestionarTrazas { get; set; }

        [Description("DESCPERMISOCONFIGURACIONES")]
        [Section("CONFIGURACION")]
        public bool GestionarConfiguraciones { get; set; }

        [Description("DESCPERMISOCACHE")]
        [Section("CONFIGURACION")]
        public bool GestionarCache { get; set; }

        [Description("DESCPERMISOSEO")]
        [Section("CONFIGURACION")]
        public bool AdministrarSEOYGoogleAnalytics { get; set; }

        [Description("DESCPERMISOESTADISTICAS")]
        [Section("CONFIGURACION")]
        public bool AccederAEstadisticasDeLaComunidad { get; set; }

        [Description("DESCPERMISOCLAUSULAS")]
        [Section("CONFIGURACION")]
        public bool GestionarClausulasDeRegistro { get; set; }

        [Description("DESCPERMISOCORREO")]
        [Section("CONFIGURACION")]
        public bool GestionarBuzonDeCorreo { get; set; }

        [Description("DESCPERMISOSERVICIOSEXTERNOS")]
        [Section("CONFIGURACION")]
        public bool GestionarServiciosExternos { get; set; }

        [Description("DESCPERMISOESTADOSERVICIOS")]
        [Section("CONFIGURACION")]
        public bool AccederAlEstadoDeLosServicios { get; set; }

        [Description("DESCPERMISOOPCIONESMETA")]
        [Section("CONFIGURACION")]
        public bool GestionarOpcionesDelMetaadministrador { get; set; }

        [Description("DESCPERMISOEVENTOS")]
        [Section("CONFIGURACION")]
        public bool GestionarEventosExternos { get; set; }

        // GRAFO
        [Description("DESCPERMISOSPARQL")]
        [Section("GRAFO")]
        public bool AccesoSparqlEndpoint { get; set; }

        [Description("DESCPERMISOCARGAMASIVA")]
        [Section("GRAFO")]
        public bool ConsultarCargasMasivas { get; set; }

        [Description("DESCPERMISOBORRADOMASIVO")]
        [Section("GRAFO")]
        public bool EjecutarBorradoMasivo { get; set; }

        [Description("DESCPERMISOSUGERENCIASBUSQUEDA")]
        [Section("GRAFO")]
        public bool GestionarSugerenciasDeBusqueda { get; set; }

        [Description("DESCPERMISOCONTEXTOS")]
        [Section("GRAFO")]
        public bool GestionarInformacionContextual { get; set; }

        // DESCUBRIMIENTO
        [Description("DESCPERMISOSEARCHPERSONALIZADO")]
        [Section("DESCUBRIMIENTO")]
        public bool GestionarParametrosDeBusquedaPersonalizados { get; set; }

        [Description("DESCPERMISOMAPA")]
        [Section("DESCUBRIMIENTO")]
        public bool GestionarMapa { get; set; }

        [Description("DESCPERMISOGRAFICOS")]
        [Section("DESCUBRIMIENTO")]
        public bool AdministrarGraficos { get; set; }

        // APARIENCIA
        [Description("DESCPERMISOVISTAS")]
        [Section("APARIENCIA")]
        public bool GestionarVistas { get; set; }

        // IC
        [Description("DESCPERMISOIC")]
        [Section("IC")]
        public bool GestionarIntegracionContinua { get; set; }

        // MANTENIMIENTO
        [Description("DESCPERMISOREPROCESAR")]
        [Section("MANTENIMIENTO")]
        public bool EjecutarReprocesadosDeRecursos { get; set; }

        // APLICACIONES
        [Description("DESCPERMISOAPLICACIONES")]
        [Section("APLICACIONES")]
        public bool GestionarAplicacionesEspecificas { get; set; }
    }
    #endregion

    # region PERMISOS DE CONTENIDOS
    public class PermisosContenidosDTO
    {
        // COMUNIDAD
        [Description("DESCPERMISOVERCATEGORIA")]
        [Section("COMUNIDAD")]
        public bool VerCategorias { get; set; }

        [Description("DESCPERMISOANYADIRCATEGORIA")]
        [Section("COMUNIDAD")]
        public bool AnyadirCategoria { get; set; }

        [Description("DESCPERMISOEDITARCATEGORIA")]
        [Section("COMUNIDAD")]
        public bool ModificarCategoria { get; set; }

        [Description("DESCPERMISOELIMINARCATEGORIA")]
        [Section("COMUNIDAD")]
        public bool EliminarCategoria { get; set; }

        // ESTRUCTURA
        [Description("DESCPERMISOVERPAGINA")]
        [Section("ESTRUCTURA")]
        public bool VerPagina { get; set; }

        [Description("DESCPERMISOCREARPAGINA")]
        [Section("ESTRUCTURA")]
        public bool CrearPagina { get; set; }

        [Description("DESCPERMISOPUBLICARPAGINA")]
        [Section("ESTRUCTURA")]
        public bool PublicarPagina { get; set; }

        [Description("DESCPERMISOEDITARPAGINA")]
        [Section("ESTRUCTURA")]
        public bool EditarPagina { get; set; }

        [Description("DESCPERMISOELIMINARPAGINA")]
        [Section("ESTRUCTURA")]
        public bool EliminarPagina { get; set; }

        [Description("DESCPERMISOVERCMS")]
        [Section("ESTRUCTURA")]
        public bool VerComponenteCMS { get; set; }

        [Description("DESCPERMISOCREARCMS")]
        [Section("ESTRUCTURA")]
        public bool CrearComponenteCMS { get; set; }

        [Description("DESCPERMISOEDITARCMS")]
        [Section("ESTRUCTURA")]
        public bool EditarComponenteCMS { get; set; }

        [Description("DESCPERMISOELIMINARCMS")]
        [Section("ESTRUCTURA")]
        public bool EliminarComponenteCMS { get; set; }

        [Description("DESCPERMISOMULTIMEDIACMS")]
        [Section("ESTRUCTURA")]
        public bool GestionarMultimediaCMS { get; set; }

        [Description("DESCPERMISORESTAURARVERSIONCMS")]
        [Section("ESTRUCTURA")]
        public bool RestaurarVersionCMS { get; set; }

        [Description("DESCPERMISOELIMINARVERSIONCMS")]
        [Section("ESTRUCTURA")]
        public bool EliminarVersionCMS { get; set; }

        [Description("DESCPERMISORESTAURARVERSIONPAGINA")]
        [Section("ESTRUCTURA")]
        public bool RestaurarVersionPagina { get; set; }

        [Description("DESCPERMISOELIMINARVERSIONPAGINA")]
        [Section("ESTRUCTURA")]
        public bool EliminarVersionPagina { get; set; }

        // GRAFO
        [Description("DESCPERMISOGESTIONAROC")]
        [Section("GRAFO")]
        public bool GestionarOC { get; set; }

        [Description("DESCPERMISOANYADIRSECUNDARIA")]
        [Section("GRAFO")]
        public bool AnyadirValorEntidadSecundaria { get; set; }

        [Description("DESCPERMISOMODIFICARSECUNDARIA")]
        [Section("GRAFO")]
        public bool ModificarValorEntidadSecundaria { get; set; }

        [Description("DESCPERMISOELIMINARSECUNDARIA")]
        [Section("GRAFO")]
        public bool EliminarValorEntidadSecundaria { get; set; }

        [Description("DESCPERMISOVERTESAURO")]
        [Section("GRAFO")]
        public bool VerTesauroSemantico { get; set; }

        [Description("DESCPERMISOANYADIRTESAURO")]
        [Section("GRAFO")]
        public bool AnyadirValorTesauro { get; set; }

        [Description("DESCPERMISOMODIFICARTESAURO")]
        [Section("GRAFO")]
        public bool ModificarValorTesauro { get; set; }

        [Description("DESCPERMISOELIMINARTESAURO")]
        [Section("GRAFO")]
        public bool EliminarValorTesauro { get; set; }

        // DESCUBRIMIENTO
        [Description("DESCPERMISOVERFACETA")]
        [Section("DESCUBRIMIENTO")]
        public bool VerFaceta { get; set; }

        [Description("DESCPERMISOCREARFACETA")]
        [Section("DESCUBRIMIENTO")]
        public bool CrearFaceta { get; set; }

        [Description("DESCPERMISOMODIFICARFACETA")]
        [Section("DESCUBRIMIENTO")]
        public bool ModificarFaceta { get; set; }

        [Description("DESCPERMISOELIMINARFACETA")]
        [Section("DESCUBRIMIENTO")]
        public bool EliminarFaceta { get; set; }
    }
    #endregion

    #region PERMISOS ECOSISTEMA
    public class PermisosEcosistemaDTO
    {
        // ECOSISTEMA
        [Description("DESCPERMISOECOSISTEMATRADUCCIONES")]
        [Section("ECOSISTEMA")]
        public bool GestionarTraduccionesEcosistema { get; set; }

        [Description("DESCPERMISOECOSISTEMADATOSEXTRA")]
        [Section("ECOSISTEMA")]
        public bool GestionarDatosExtraRegistroEcosistema { get; set; }

        [Description("DESCPERMISOECOSISTEMACORREO")]
        [Section("ECOSISTEMA")]
        public bool GestionarBuzonDeCorreoEcosistema { get; set; }

        [Description("DESCPERMISOECOSISTEMAEVENTOS")]
        [Section("ECOSISTEMA")]
        public bool GestionarEventosExternosEcosistema { get; set; }

        [Description("DESCPERMISOECOSISTEMACATEGORIAS")]
        [Section("ECOSISTEMA")]
        public bool GestionarCategoriasDePlataforma { get; set; }

        [Description("DESCPERMISOECOSISTEMACONFIGURACION")]
        [Section("ECOSISTEMA")]
        public bool GestionarLaConfiguracionPlataforma { get; set; }

        [Description("DESCPERMISOECOSISTEMASHAREPOINT")]
        [Section("ECOSISTEMA")]
        public bool ConfiguracionDeSharePoint { get; set; }

        [Description("DESCPERMISOECOSISTEMAVISTAS")]
        [Section("ECOSISTEMA")]
        public bool GestionarVistasEcosistema { get; set; }

        [Description("DESCPERMISOECOSISTEMAIC")]
        [Section("ECOSISTEMA")]
        public bool AdministrarIntegracionContinua { get; set; }

        [Description("DESCPERMISOECOSISTEMASOLICITUDES")]
        [Section("ECOSISTEMA")]
        public bool AdministrarSolicitudesComunidad { get; set; }

        [Description("DESCPERMISOECOSISTEMAROLES")]
        [Section("ECOSISTEMA")]
        public bool GestionarRolesYPermisosEcosistema { get; set; }

        [Description("DESCPERMISOECOSISTEMAMIEMBROS")]
        [Section("ECOSISTEMA")]
        public bool AdministrarMiembrosEcosistema { get; set; }
    }
    #endregion

    #region PERMISOS RECURSOS
    public class PermisosRecursosDTO
    {
        // RECURSOS - Adjuntos
        [Description("DESCPERMISOCREARADJUNTO")]
        [Section("RECURSOS")]
        public bool CrearRecursoTipoAdjunto { get; set; }

        [Description("DESCPERMISOEDITARADJUNTO")]
        [Section("RECURSOS")]
        public bool EditarRecursoTipoAdjunto { get; set; }

        [Description("DESCPERMISOELIMINARADJUNTO")]
        [Section("RECURSOS")]
        public bool EliminarRecursoTipoAdjunto { get; set; }

        // RECURSOS - Referencias
        [Description("DESCPERMISOCREARREFERENCIA")]
        [Section("RECURSOS")]
        public bool CrearRecursoTipoReferenciaADocumentoFisico { get; set; }

        [Description("DESCPERMISOEDITARREFERENCIA")]
        [Section("RECURSOS")]
        public bool EditarRecursoTipoReferenciaADocumentoFisico { get; set; }

        [Description("DESCPERMISOELIMINARREFERNCIA")]
        [Section("RECURSOS")]
        public bool EliminarRecursoTipoReferenciaADocumentoFisico { get; set; }

        // RECURSOS - Enlaces
        [Description("DESCPERMISOCREARENLACE")]
        [Section("RECURSOS")]
        public bool CrearRecursoTipoEnlace { get; set; }

        [Description("DESCPERMISOEDITARENLACE")]
        [Section("RECURSOS")]
        public bool EditarRecursoTipoEnlace { get; set; }

        [Description("DESCPERMISOELIMINARENLACE")]
        [Section("RECURSOS")]
        public bool EliminarRecursoTipoEnlace { get; set; }

        // RECURSOS - Notas
        [Description("DESCPERMISOCREARNOTA")]
        [Section("RECURSOS")]
        public bool CrearNota { get; set; }

        [Description("DESCPERMISOEDITARNOTA")]
        [Section("RECURSOS")]
        public bool EditarNota { get; set; }

        [Description("DESCPERMISOELIMINARNOTA")]
        [Section("RECURSOS")]
        public bool EliminarNota { get; set; }

        // RECURSOS - Preguntas
        [Description("DESCPERMISOCREARPREGUNTA")]
        [Section("RECURSOS")]
        public bool CrearPregunta { get; set; }

        [Description("DESCPERMISOEDITARPREGUNTA")]
        [Section("RECURSOS")]
        public bool EditarPregunta { get; set; }

        [Description("DESCPERMISOELIMINARPREGUNTA")]
        [Section("RECURSOS")]
        public bool EliminarPregunta { get; set; }

        // RECURSOS - Encuestas
        [Description("DESCPERMISOCREARENCUESTA")]
        [Section("RECURSOS")]
        public bool CrearEncuesta { get; set; }

        [Description("DESCPERMISOEDITARENCUESTA")]
        [Section("RECURSOS")]
        public bool EditarEncuesta { get; set; }

        [Description("DESCPERMISOELIMINARENCUESTA")]
        [Section("RECURSOS")]
        public bool EliminarEncuesta { get; set; }

        // RECURSOS - Debates
        [Description("DESCPERMISOCREARDEBATE")]
        [Section("RECURSOS")]
        public bool CrearDebate { get; set; }

        [Description("DESCPERMISOEDITARDEBATE")]
        [Section("RECURSOS")]
        public bool EditarDebate { get; set; }

        [Description("DESCPERMISOELIMINARDEBATE")]
        [Section("RECURSOS")]
        public bool EliminarDebate { get; set; }

        // RECURSOS - Semánticos
        [Description("DESCPERMISOCREARSEMANTICO")]
        [Section("RECURSOS")]
        public bool CrearRecursoSemantico { get; set; }

        [Description("DESCPERMISOEDITARSEMANTICO")]
        [Section("RECURSOS")]
        public bool EditarRecursoSemantico { get; set; }

        [Description("DESCPERMISOELIMINARSEMANTICO")]
        [Section("RECURSOS")]
        public bool EliminarRecursoSemantico { get; set; }

        // RECURSOS - Versiones Enlaces
        [Description("DESCPERMISORESTAURARVERSIONENLACE")]
        [Section("RECURSOS")]
        public bool RestaurarVersionEnlace { get; set; }

        [Description("DESCPERMISOELIMINARVERSIONENLACE")]
        [Section("RECURSOS")]
        public bool EliminarVersionEnlace { get; set; }

        // RECURSOS - Versiones Adjuntos
        [Description("DESCPERMISORESTAURARVERSIONADJUNTO")]
        [Section("RECURSOS")]
        public bool RestaurarVersionAdjunto { get; set; }

        [Description("DESCPERMISOELIMINARVERSIONADJUNTO")]
        [Section("RECURSOS")]
        public bool EliminarVersionAdjunto { get; set; }

        // RECURSOS - Versiones Referencias
        [Description("DESCPERMISORESTAURARVERSIONREFERNCIA")]
        [Section("RECURSOS")]
        public bool RestaurarVersionReferencia { get; set; }

        [Description("DESCPERMISOELIMINARVERSIONREFERNCIA")]
        [Section("RECURSOS")]
        public bool EliminarVersionReferencia { get; set; }

        // RECURSOS - Versiones Notas
        [Description("DESCPERMISORESTAURARVERSIONNOTA")]
        [Section("RECURSOS")]
        public bool RestaurarVersionNota { get; set; }

        [Description("DESCPERMISORESELIMINARVERSIONNOTA")]
        [Section("RECURSOS")]
        public bool EliminarVersionNota { get; set; }

        // RECURSOS - Versiones Preguntas
        [Description("DESCPERMISORESTAURARVERSIONPREGUNTA")]
        [Section("RECURSOS")]
        public bool RestaurarVersionPregunta { get; set; }

        [Description("DESCPERMISOELIMINARVERSIONPREGUNTA")]
        [Section("RECURSOS")]
        public bool EliminarVersionPregunta { get; set; }

        // RECURSOS - Versiones Encuestas
        [Description("DESCPERMISORESTAURARVERSIONENCUESTA")]
        [Section("RECURSOS")]
        public bool RestaurarVersionEncuesta { get; set; }

        [Description("DESCPERMISOELIMINARVERSIONENCUESTA")]
        [Section("RECURSOS")]
        public bool EliminarVersionEncuesta { get; set; }

        // RECURSOS - Versiones Debates
        [Description("DESCPERMISORESTAURARVERSIONDEBATE")]
        [Section("RECURSOS")]
        public bool RestaurarVersionDebate { get; set; }

        [Description("DESCPERMISOELIMINARVERSIONDEBATE")]
        [Section("RECURSOS")]
        public bool EliminarVersionDebate { get; set; }

        // RECURSOS - Certificación
        [Description("DESCPERMISOCERTIFICARRECURSO")]
        [Section("RECURSOS")]
        public bool CertificarRecurso { get; set; }
    }
    #endregion

}
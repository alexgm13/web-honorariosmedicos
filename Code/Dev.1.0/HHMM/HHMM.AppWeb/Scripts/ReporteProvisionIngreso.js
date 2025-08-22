var $ = {
	ajax: function (url, type, success, text) {
		requestServer(url, type, success, text);
	}
};
function requestServer(url, type, success, text) {
	var xhr = new XMLHttpRequest();
	xhr.open(type, urlBase + url);
	xhr.onreadystatechange = function () {
		if (xhr.status == 200 && xhr.readyState == 4) {
			if (xhr.responseText.length >= 6 && xhr.responseText.substr(0, 6) == "reload")
				window.parent.parent.location.reload();
			success(xhr.responseText);
		}
	}
	if (type == "get") xhr.send();
	else {
		if (text != null && text != "") xhr.send(text);
	}
}

var urlBase = "", ss = "";
var sucursalId;
var _cabeceras = [
	"Compañía", "Sucursal", "Cliente Facturación",
	"Documento Cliente", "Nombre Cliente", "Dirección Cliente",
	"Tipo Comprobante", "Referencia", "Línea",
	"Tipo Componente", "Componente", "Nombre Componente",
	"Cantidad", "Moneda", "Tipo Cambio",
	"Precio Unitario", "Monto Afecto", "Monto No Afecto",
	"Monto Impuesto Ventas", "Clasificador Movimiento", "Unidad Organizacional",
	"Concepto Gasto", "Centro Costo", "Sistema Fuente Carga",
	"Proceso Carga", "Fecha Carga", "Usuario Responsable Carga",
	"Estado Carga", "Sistema Fuente", "Proceso",
	"Fecha", "Usuario Responsable", "Estado",
	"Indicador Impuesto", "Documento", "Número Voucher",
	"Ítems del Voucher"
];
var _anchos = [
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7, 2.7, 2.7,
	2.7
];
var _anchosExportar = [
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100, 100, 100,
	100
];
var _indices = [
	0, 1, 2,
	3, 4, 5,
	6, 7, 8,
	9, 10, 11,
	12, 13, 14,
	15, 16, 17,
	18, 19, 20,
	21, 22, 23,
	24, 25, 26,
	27, 28, 29,
	30, 31, 32,
	33, 34, 35,
	36
];
var _tipoFiltro = [
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I', 'I', 'I',
	'I'
];
var _idFiltro = [
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	'', '', '',
	''
];
var _registrosPagina = 10, _indiceActualPagina = 0;
var _paginasBloque = 5, _indiceActualBloque = 0;
var _registrosSeleccionados = [];

var contenidoExportar = "";
var _data, _matriz = [];

window.onload = function () {
	urlBase = location.protocol + "//" + window.location.host + document.getElementById("url").value;
	ss = window.parent.document.getElementById("iss").value;
	//var hdfSucursal = document.getElementById("hdfSucursal").value;
	//if (hdfSucursal != "") {
	//	var elemento1 = document.getElementsByClassName("fa-search")[0];
	//	var elemento2 = document.getElementsByClassName("fa-search")[1];
	//	elemento1.style.display = "none";
	//	elemento2.style.display = "none";
	//	var cboTurno = document.getElementById("cboTurno");
	//	cboTurno.className += " lectura";
	//	cboTurno.readOnly = "readOnly";
	//	cboTurno.disabled = true;
	//	var btnLimpiar = document.getElementById("btnLimpiar");
	//	btnLimpiar.style.display = "none";
	//}
	cargaInicial();
};

function cargaInicial() {
	sucursalId = window.parent.document.getElementById("isuc").value.split("|")[0];
	//var fecha = new Date();
	//var cboPeriodo = document.getElementById("cboPeriodo");
	//var txtAnio = document.getElementById("txtAnio");
	//cboPeriodo.value = fecha.getMonth() + 1;
	//txtAnio.value = fecha.getFullYear();
	crearTabla();
	configurarFiltro();
	configurarBotones();
	$.ajax("Control/ReporteProvisionIngreso_Listas/?ss=" + ss, "get", cargarDatos);
}


function crearTabla() {
	var nCampos = _cabeceras.length;
	var contenido = "<table class='tabla-general' style='width:400%;max-width:max-content;'>";
	contenido += "<thead class='tabla-FilaCab'>";
	contenido += "<tr class='cabecera'>";
	for (var j = 0; j < nCampos; j++) {
		contenido += "<th style='width:";
		contenido += _anchos[j];
		contenido += "%;text-align:center'><span id='spn";
		contenido += j.toString();
		//contenido += "' class='Enlace' data-orden='";
		//contenido += matrizIndice[j];
		contenido += "' style='cursor:default;'>";
		contenido += _cabeceras[j];
		contenido += "</span>";
		contenido += "<br/>";

		if (_tipoFiltro[j]) {
			if (_tipoFiltro[j] == 'I') {
				contenido += "<input type='text' class='Texto' name='cabecera' style='width:90%'>";
			} else {
				contenido += "<select id='" + _idFiltro[j] + "' class='Combo' name='cabecera' style='width:90%'></select>";
			}
		}

		contenido += "</th>";
	}
	contenido += "</tr>";
	contenido += "</thead>";
	contenido += "<tbody id='tbResultado' class='tabla-FilaCuerpo'>";
	contenido += "</tbody>";
	contenido += "<tfoot>";
	contenido += "<tr><td id='tdPaginas' colspan='";
	contenido += (nCampos).toString();
	contenido += "'></td></tr>";
	contenido += "</tfoot>";
	contenido += "</table>";
	document.getElementById("divResultado").innerHTML = contenido;
}


function cargarDatos(rpta) {
	if (rpta != "") {
		var data = rpta.split("¯");
		llenarCombo(data[0], "cboSucursal", "¬", "¦", "Todos");
		document.getElementById('cboSucursal').value = sucursalId;
		//var hdfMedico = document.getElementById("hdfMedico");
		//var hdfFecha = document.getElementById("hdfFecha");
		//if (hdfMedico.value != "" && hdfMedico.value != "-1") {

		//	document.getElementById("txtMedico").value = (document.getElementById("hdfMedico").value == "-1" ? "" : document.getElementById("hdfMedico").value.split("¦")[1]);
		//	if (hdfFecha.value != "") {
		//		document.getElementById("cboPeriodo").value = parseInt(hdfFecha.value.split("¦")[0]);
		//		document.getElementById("txtAnio").value = hdfFecha.value.split("¦")[1];
		//	}
		//	document.getElementById("divbtnRegresar").style.display = "";
		//	document.getElementById("divultimo").style.display = "";
		//	document.getElementById("btnRegresar").style.display = "";
		//	document.getElementById("btnBuscar").click();
		//}
	}
}


function llenarCombo(lista, nombreCombo, separadorRegistro, separadorCampo, inicio) {
	var contenido = "";
	var listas = lista.split(separadorRegistro);
	var n = listas.length;
	var valor = "";
	var campos = "";
	//var hdfSucursal = document.getElementById("hdfSucursal").value;
	//contenido = "<option value=''>" + inicio + "</option>";
	//if (nombreCombo == "cboUnidadMedica") {
	//	for (var x = 0; x < n; x++) {
	//		campos = listas[x].split(separadorCampo);
	//		contenido += "<option value='" + campos[0] + "'>" + campos[1] + "</option>";
	//	}
	//}
	//else {
	//	for (var x = 0; x < n; x++) {
	//		campos = listas[x].split(separadorCampo);
	//		if (hdfSucursal != "" && campos[2] == hdfSucursal) {
	//		}
	//		else {
	//			if (hdfSucursal == "" && campos[2] == sucursalId) {
	//				contenido += "<option value='" + campos[0] + "'>" + campos[1] + "</option>";
	//			}
	//		}
	//	}
	//}
	if (inicio) {
		contenido = "<option value=''>" + inicio + "</option>";
    }
	for (var x = 0; x < n; x++) {
		campos = listas[x].split(separadorCampo);
		contenido += "<option value='" + campos[0] + "'>" + campos[1] + "</option>";
	}

	var cbo = document.getElementById(nombreCombo);
	if (cbo != null) {
		cbo.innerHTML = contenido;
	}
}


function configurarBotones() {
	document.getElementById("btnBuscar").onclick = function () {

		document.getElementById('divResultado').style.display = ''
		document.getElementById("tbResultado").innerHTML = "";
		//var sucursal = window.parent.document.getElementById("isuc").value.split("|")[0];
		var cboSucursal = document.getElementById("cboSucursal").value;
		var cboPeriodo = document.getElementById("cboPeriodo").value;
		var txtAnio = document.getElementById("txtAnio").value;
		var txtMedico = document.getElementById("hdfMedico").value.split("¦")[0];
		var txtComponente = document.getElementById("hdfCom").value.split("¦")[0];
		var txtCentroCosto = document.getElementById("txtCentroCosto").value;
		var mensaje = cboSucursal + "¦" + cboPeriodo + "¦" + txtAnio + "¦" + ((txtMedico == "" || txtMedico == "-1") ? "" : txtMedico) + "¦" + ((txtComponente == "" || txtComponente == "-1") ? "" : txtComponente) + "¦" + txtCentroCosto;
		$.ajax("Control/ReporteProvisionIngreso_Listar/?ss=" + ss, "post", mostrarResultado, mensaje);
	};

	var spnDoctor = document.getElementById("spnDoctor");
	spnDoctor.onclick = function () {
		var ifrMedico = document.getElementById("ifrMedico");
		if (ifrMedico.innerHTML == "") {
			ifrMedico.innerHTML = "<iframe style='margin:0;padding:0;width:950px;height:500px;border: 1px solid transparent;' src='" + urlBase + "Mantenimiento/MedicoLista/?ss=" + ss + "'></iframe>";
		}
		abrirPopup("PopupMedico");
	}


	var spnComponente = document.getElementById("spnComponente");
	spnComponente.onclick = function () {
		var ifrComponente = document.getElementById("ifrComponente");
		if (ifrComponente.innerHTML == "") {
			ifrComponente.innerHTML = "<iframe style='margin:0;padding:0;width:700px;height:500px;border: 1px solid transparent;' src='" + urlBase + "Mantenimiento/ComponenteLista/?ss=" + ss + "'></iframe>";
		}
		abrirPopup("PopupComponente");
	}

	var hdfMedico = document.getElementById("hdfMedico");
	hdfMedico.onchange = function () {
		if (this.value != "-1") {
			var datos = this.value.split("¦");
			this.value = datos[0];
			var txtMedico = document.getElementById("txtMedico");
			txtMedico.value = datos[1];
		}
	}

	var hdfCom = document.getElementById("hdfCom");
	hdfCom.onchange = function () {
		if (this.value != "-1") {
			var datos = this.value.split("¦");
			this.value = datos[0];
			var txtComponente = document.getElementById("txtComponente");
			txtComponente.value = datos[1];
		}
	}

	var btnLimpiar = document.getElementById("btnLimpiar");
	btnLimpiar.onclick = function () {
		var limpiar = document.getElementsByClassName("limpiar");
		for (var y = 0; y < limpiar.length; y++) {
			if (limpiar[y].type == "checkbox") {
				limpiar[y].checked = false;
			} else {
				if (limpiar[y].nodeName == "SELECT") {
					limpiar[y].selectedIndex = "0";
				}
				else {
					limpiar[y].value = "";
				}
			}

		}
		document.getElementById("hdfMedico").value = "-1";
		document.getElementById("hdfCom").value = "-1";
	}

	var btnExportar = document.getElementById("btnExportar");
	btnExportar.onclick = function () {
		var nRegistros = _matriz.length;
		if (nRegistros > 0) {
			exportacion(this);
		} else {
			this.removeAttribute("download");
			this.href = "#";
		}
	}
}


function crearMatriz() {
	var nRegistros = _data.length;
	var nCampos;
	var campos;
	var x = 0;

	if (nRegistros > 0) {
		_matriz = [];
		for (i = 0; i < nRegistros; i++) {
			campos = _data[i].split("¦");
			_matriz[x] = [];
			nCampos = campos.length;
			for (j = 0; j < nCampos; j++) {
				if (isNaN(campos[j]) || j == 0 || j == 2 || j == 3) _matriz[x][j] = campos[j];
				else _matriz[x][j] = campos[j] * 1;
			}
			x++;
		}
	}
	else {
		_matriz = [];
	}
}

function mostrarResultado(rpta) {
	_data = rpta ? rpta.split("¬") : [];

	crearMatriz();
	paginar(0);
}

function paginar(indicePagina) {
	var nRegistros = _matriz.length;
	var esBloque = (indicePagina < 0);
	if (esBloque) {
		var indiceUltimaPagina = Math.floor(nRegistros / _registrosPagina);
		if (nRegistros % _registrosPagina == 0) indiceUltimaPagina--;
		var indiceUltimoBloque = Math.floor(nRegistros / (_paginasBloque * _registrosPagina));
		if (nRegistros % (_paginasBloque * _registrosPagina) == 0) indiceUltimoBloque--;
		switch (indicePagina) {
			case -1:
				indicePagina = 0;
				_indiceActualBloque = 0;
				break;
			case -2:
				if (_indiceActualBloque > 0) {
					_indiceActualBloque--;
					indicePagina = _indiceActualBloque * _paginasBloque;
				}
				break;
			case -3:
				if (_indiceActualBloque < indiceUltimoBloque) {
					_indiceActualBloque++;
					indicePagina = _indiceActualBloque * _paginasBloque;
				}
				break;
			case -4:
				indicePagina = indiceUltimaPagina;
				_indiceActualBloque = indiceUltimoBloque;
				break;
		}
	}
	_indiceActualPagina = indicePagina;
	mostrarMatriz(indicePagina);
}


function mostrarMatriz(indicePagina) {
	_indiceActualPagina = indicePagina;
	var contenido = "";
	//Campoeliminar = "";
	var n = _matriz.length;
	var nCabeceras = _cabeceras.length;
	//var ConfiguracionActual = 0;
	if (n > 0) {
		var nCampos = _cabeceras.length;
		var inicio = indicePagina * _registrosPagina;
		var fin = inicio + _registrosPagina;
		for (var i = inicio; i < fin; i++) {
			if (i < n) {
				contenido += "<tr class='FilaDatos'>";
				for (var j = 0; j < nCampos; j++) {
					switch (j) {
						case 15:
						case 16:
						case 17:
						case 18:
							contenido += "<td style='text-align:right'>";
							contenido += _matriz[i][_indices[j]].toLocaleString('en-US', { minimumFractionDigits: 2 });
							contenido += "</td>";
							break;
						default:
							contenido += "<td>";
							contenido += _matriz[i][_indices[j]];
							contenido += "</td>";
							break;
					}

				}
				contenido += "</tr>";
			}

		}
		//contenido += "<tr><td style='text-align:right;font-weight:bold' colspan='3'>Totales</td><td style='text-align:right'>" + formatearNumero(_totales.mtImporte) + "</td><td style='text-align:right'>" + formatearNumero(_totales.mtBonificacion) + "</td><td style='text-align:right'>" + formatearNumero(_totales.mtDescuento) + "</td><td style='text-align:right'>" + formatearNumero(_totales.mtAjuste) + "</td><td style='text-align:right'>" + formatearNumero(_totales.mtTotal) + "</td><td colspan='2'></td></tr>";
	}
	else {
		contenido += "<tr class='FilaDatos'><td style='text-align:center' colspan='";
		contenido += (nCabeceras + 0).toString();
		contenido += "'>No hay datos</td></tr>";
	}
	//excelExportarDetalle = contenido;
	document.getElementById("tbResultado").innerHTML = contenido;
	crearPaginas();
}


function crearPaginas() {
	var nRegistros = _matriz.length;
	var indiceUltimaPagina = Math.floor(nRegistros / _registrosPagina);
	if (nRegistros % _registrosPagina == 0) indiceUltimaPagina--;
	var indiceUltimoBloque = Math.floor(nRegistros / (_paginasBloque * _registrosPagina));
	if (nRegistros % (_paginasBloque * _registrosPagina) == 0) indiceUltimoBloque--;
	var contenido = "";
	var inicio = _indiceActualBloque * _paginasBloque;
	var fin = inicio + _paginasBloque;
	if (_indiceActualBloque > 0 && nRegistros > (_paginasBloque * _registrosPagina)) {
		contenido += "<span class='pagina' onclick='paginar(-1);' title='Ir al primer grupo de páginas'>&lt;&lt;</span>";
		contenido += "<span class='pagina' onclick='paginar(-2);' title='Ir al anterior grupo de páginas'>&lt;</span>";
	}
	for (var i = inicio; i < fin; i += 1) {
		if (i <= indiceUltimaPagina) {
			contenido += "<span onclick='paginar(";
			contenido += i;
			contenido += ");'  title='Ir a la pagina ";
			contenido += (i + 1).toString();
			contenido += "' id='a";
			contenido += i.toString();
			contenido += "' class='pagina' >";
			contenido += (i + 1).toString();
			contenido += "</span>";

		} else break;
	}
	if (_indiceActualBloque < indiceUltimoBloque && nRegistros > (_paginasBloque * _registrosPagina)) {
		contenido += "<span class='pagina' onclick='paginar(-3);' title='Ir al siguiente grupo de páginas'>&gt;</span>";
		contenido += "<span class='pagina' onclick='paginar(-4);' title='Ir al último grupo de páginas'>&gt;&gt;</span>";
	}
	if (nRegistros <= _registrosPagina) {
		document.getElementById("tdPaginas").innerHTML = "";
	}
	else {
		document.getElementById("tdPaginas").innerHTML = contenido;
		seleccionarPaginaActual();
	}
}

function seleccionarPaginaActual(dato) {
	var indice;
	indice = _indiceActualPagina;
	var aPagina = document.getElementById("a" + indice);
	if (aPagina != null) {
		aPagina.className += " seleccionado";
	}
}

function configurarFiltro() {
	var textos = document.getElementsByClassName("Texto");
	var ntextos = textos.length;
	var texto;
	for (i = 0; i < ntextos; i++) {
		texto = textos[i];
		texto.onkeyup = function (e) {
			filtrar(true);
		}
	}
	var combos = document.getElementsByClassName("Combo");
	var nCombos = combos.length;
	var combo;
	for (i = 0; i < nCombos; i++) {
		combo = combos[i];
		combo.onchange = function (e) {
			filtrar(true);
		}
	}
}


function filtrar() {
	var cabeceras = document.getElementsByName("cabecera");
	var nCabeceras = cabeceras.length;
	var cabecera;
	var exito;
	_matriz = [];
	var nRegistros = _data.length;
	var nCampos;
	var campos;
	var campoFiltrado = [];
	var x = 0;
	var nFiltrados = _indices.length

	for (var i = 0; i < nRegistros; i++) {
		campos = _data[i].split("¦");
		campoFiltrado = [];
		nCampos = campos.length;
		for (var k = 0; k < nFiltrados; k++) {
			campoFiltrado.push(campos[_indices[k]]);
		}

		for (var j = 0; j < nCabeceras; j++) {
			exito = true;
			cabecera = cabeceras[j];
			if (cabecera.className == "Texto") exito = exito && (campoFiltrado[j].toLowerCase().trim().indexOf(cabecera.value.toLowerCase().trim()) > -1);
			else exito = exito && (cabecera.value == "" || campoFiltrado[j].toLowerCase() == cabecera.value.toLowerCase());
			if (!exito) break;
		}

		if (exito) {
			_matriz[x] = [];
			nCampos = campos.length;
			for (j = 0; j < nCampos; j++) {
				if (isNaN(campos[j]) || j == 1) {
					_matriz[x][j] = campos[j];
				}
				else {
					_matriz[x][j] = campos[j] * 1;
				}

			}
			x++;
		}

	}
	paginar(0);
	_indiceActualBloque = 0;
}

function exportacion(objeto) {
	var contenido = "";
	var nRegistros = _matriz.length;
	if (nRegistros > 0) {
		contenido = crearCabeceraExportar();
		var nCampos = _cabeceras.length;
		for (var i = 0; i < nRegistros; i++) {
			contenido += "<tr>";
			for (var j = 0; j < nCampos; j++) {
				switch (j) {
					case 15:
					case 16:
					case 17:
					case 18:
						contenido += "<td style='text-align:right' class='number_2'>";
						contenido += _matriz[i][_indices[j]].toLocaleString('en-US', { minimumFractionDigits: 2 });
						contenido += "</td>";
						break;
					default:
						contenido += "<td style='text-align:left' class='string'>";
						contenido += _matriz[i][_indices[j]];
						contenido += "</td>";
						break;
				}
			}
			contenido += "</tr>";
		}
		contenido += "</table></body></html>";
		var formBlob = new Blob([contenido], {
			encoding: "UTF-8", type: 'application/vnd.ms-excel;charset=UTF-8'
		});
		objeto.download = "ReporteProvisionIngreso.xls";
		objeto.href = window.URL.createObjectURL(formBlob);
	}
}

function crearCabeceraExportar() {
	var cabecera = "<html><head><meta charset='utf-8'/>";
	cabecera += '<style>.number_2{ mso-number-format:"0\.00"} .string{ mso-number-format:"\\@"}</style>';
	cabecera += "</head>";
	cabecera += "<body><table>";
	cabecera += "<tr style='background-Color:\"#00a850\"; color:\"\#FFF\"'>";
	for (let i = 0; i < _cabeceras.length; i++) {
		cabecera += "<td style='width: " + _anchosExportar[i] + "px'>" + _cabeceras[i] + "</td>";
    }
	cabecera += "</tr>";
	return cabecera;
}

function abrirPopup(popup) {
	var popup = document.getElementById(popup);
	if (popup.className.indexOf("Open") == -1) {
		popup.className += " Open";
	} else {
		popup.className = "PopUp";
	}
}

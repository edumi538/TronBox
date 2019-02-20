// Recovery all data
'use strict';

let mongo = require('mongodb');
let assert = require('assert');
let chalk = require('chalk');
let axios = require('axios');
let winston = require('winston');
let moment = require('moment');
let crypto = require("crypto");

const tsFormat = () => (new Date()).toLocaleString();

const logger = new (winston.Logger)({
	transports: [
		new (winston.transports.Console)({
			timestamp: tsFormat,
			colorize: true,
			level: 'info'
		}),
		new (winston.transports.File)({
			filename: 'C:/Projetos/recovery-all-data.log',
			timestamp: tsFormat,
			level: process.env.NODE_ENV === 'development' ? 'debug' : 'info'
		})
	]
});

function isNumber(n) { return !isNaN(parseFloat(n)) && !isNaN(n - 0) }

function buscarTipoDia(tipoDia) {
	if (tipoDia.toUpperCase() == 'DSR'.toUpperCase())
		return 2;
	else if (tipoDia.toUpperCase() == 'Compensado'.toUpperCase())
		return 3;
	else if (tipoDia.toUpperCase() == 'Férias'.toUpperCase())
		return 4;
	else if (tipoDia.toUpperCase() == 'Afastado'.toUpperCase())
		return 5;
	else if (tipoDia.toUpperCase() == 'Recisão'.toUpperCase())
		return 6;
	else if (tipoDia.toUpperCase() == 'Licença'.toUpperCase())
		return 7;
	else if (tipoDia.toUpperCase() == 'Feriado'.toUpperCase())
		return 8;
	else if (tipoDia.toUpperCase() == 'Demitido'.toUpperCase())
		return 9;
	else
		return 1;
}

function getCodigoAfastamento(codigoAfastamento) {
	if (codigoAfastamento == '05')
		return 1;
	else if (codigoAfastamento == '07')
		return 2;
	else if (codigoAfastamento == '08')
		return 3;
	else if (codigoAfastamento == '10')
		return 4;
	else if (codigoAfastamento == '11')
		return 5;
	else if (codigoAfastamento == '12')
		return 6;
	else if (codigoAfastamento == '13')
		return 7;
	else if (codigoAfastamento == '14')
		return 8;
	else if (codigoAfastamento == '15')
		return 9;
	else if (codigoAfastamento == '16')
		return 10;
	else if (codigoAfastamento == '18')
		return 11;
	else if (codigoAfastamento == '19')
		return 12;
	else if (codigoAfastamento == '20')
		return 13;
	else if (codigoAfastamento == '22')
		return 14;
	else if (codigoAfastamento == '23')
		return 15;
	else if (codigoAfastamento == '24')
		return 16;
	else if (codigoAfastamento == '25')
		return 17;
	else if (codigoAfastamento == '26')
		return 18;
	else if (codigoAfastamento == '27')
		return 19;
	else if (codigoAfastamento == '28')
		return 20;
	else if (codigoAfastamento == '30')
		return 21;
	else if (codigoAfastamento == '31')
		return 22;
	else if (codigoAfastamento == '33')
		return 23;
	else if (codigoAfastamento == '34')
		return 24;
	else if (codigoAfastamento == 'O1')
		return 25;
	else if (codigoAfastamento == 'O2')
		return 26;
	else if (codigoAfastamento == 'O3')
		return 27;
	else if (codigoAfastamento == 'P1')
		return 28;
	else if (codigoAfastamento == 'P2')
		return 29;
	else if (codigoAfastamento == 'P3')
		return 30;
	else if (codigoAfastamento == 'Q1')
		return 31;
	else if (codigoAfastamento == 'Q2')
		return 32;
	else if (codigoAfastamento == 'Q3')
		return 33;
	else if (codigoAfastamento == 'Q4')
		return 34;
	else if (codigoAfastamento == 'Q5')
		return 35;
	else if (codigoAfastamento == 'Q6')
		return 36;
	else if (codigoAfastamento == 'R')
		return 37;
	else if (codigoAfastamento == 'U2')
		return 38;
	else if (codigoAfastamento == 'U3')
		return 39;
	else if (codigoAfastamento == 'W')
		return 40;
	else if (codigoAfastamento == 'X')
		return 41;
	else if (codigoAfastamento == 'Y')
		return 4;
}

function getAuthorization() {
	return new Promise(resolve => {
		try {
			axios({
				url: 'http://localhost:6005/api/autenticacao/login',
				method: 'POST',
				header: [{
					key: 'Content-type',
					value: 'application/json'
				}],
				data: {
					'usuario': 'carlos.aurelio@tron.com.br',
					'senha': '123456'
				}
			}).then(response => {
				resolve({
					accessToken: response.data.token,
					tenant: response.data.tenants[0].tenantId
				});
			}).catch(error => {
				resolve(null);
			});
		} catch (e) {
			resolve(null);
		}
	});
}

function requestCreateEmployee(data, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			axios({
				method: 'post',
				url: 'http://localhost:6004/api/empregados',
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'Content-Type': 'application/json',
					'ServiceIdentify': tenant
				},
				data: data
			}).then(response => {
				resolve({
					id: response.headers.location,
					sucesso: response.data.sucesso,
					message: response.data.message
				});
			}).catch(error => {
				resolve(error.response.data);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

function requestGetEmployee(registry, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			axios({
				method: 'get',
				url: `http://localhost:6004/api/empregados?filtro=[{"campo": "inscricao", "valor": "${registry}"}]`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'ServiceIdentify': tenant
				}
			}).then(response => {
				resolve(response.data.length > 0 ? response.data[0].inscricao : false);
			}).catch(error => {
				resolve(false);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

function requestGetPessoa(registry, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			axios({
				method: 'get',
				url: `http://localhost:6004/api/pessoas?description=${registry}`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'ServiceIdentify': tenant
				}
			}).then(response => {
				resolve(response.data.pessoas.length > 0 ? response.data.pessoas[0].id : false);
			}).catch(error => {
				resolve(false);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

function requestGetCargo(cargo, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			let campo = isNumber(cargo) ? 'codigo' : 'descricao'

			axios({
				method: 'get',
				url: `http://localhost:6004/api/cargos?filtro=[{"campo": "${campo}", "valor": "${cargo}"}]`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'ServiceIdentify': tenant
				}
			}).then(response => {
				resolve(response.data.length > 0 ? response.data[0].id : false);
			}).catch(error => {
				resolve(false);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

function requestGetDepartamento(departamento, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			let campo = isNumber(departamento) ? 'codigo' : 'descricao'

			axios({
				method: 'get',
				url: `http://localhost:6004/api/departamentos?filtro=[{"campo": "${campo}", "valor": "${departamento}"}]`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'ServiceIdentify': tenant
				}
			}).then(response => {
				resolve(response.data.length > 0 ? response.data[0].id : false);
			}).catch(error => {
				resolve(false);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

function requestGetHorario(descricao, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			axios({
				method: 'get',
				url: `http://localhost:6004/api/horarios-trabalho?filtro=[{"campo": "descricao", "valor": "${descricao}"}]`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'ServiceIdentify': tenant
				}
			}).then(response => {
				resolve(response.data.length > 0 ? response.data[0].id : false);
			}).catch(error => {
				resolve(false);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

function requestGetMotivo(codigo, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			axios({
				method: 'get',
				url: `http://localhost:6004/api/motivos?filtro=[{"campo": "codigo", "valor": "${codigo}"}]`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'ServiceIdentify': tenant
				}
			}).then(response => {
				resolve(response.data.length > 0 ? response.data[0].id : false);
			}).catch(error => {
				resolve(false);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

function requestCreate(endPoint, data, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			axios({
				method: 'post',
				url: `http://localhost:6004/api/${endPoint}`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'Content-Type': 'application/json',
					'ServiceIdentify': tenant
				},
				data: data
			}).then(response => {
				resolve(response.data);
			}).catch(error => {
				resolve(error.data);
			});
		} catch (e) {
			resolve(e);
		}
	});
}

async function recoveryCad(endPoint, datas, accessToken, tenant) {
	let arrayOfData = [];

	for (let data of datas) {
		let newData = {
			codigo: data.code,
			descricao: data.description
		}

		arrayOfData.push(newData);
	}

	if (arrayOfData.length > 0)
		return await requestCreate(endPoint, arrayOfData, accessToken, tenant);
}

async function recoveryCompany(company, accessToken, tenant) {
	let data = {
		razaoSocial: company.name,
		inscricao: company.registry,
		tipoInscricao: 'CNPJ',
		emailPrincipal: 'carlos.aurelio@tron.com.br'
	}

	return await requestCreate('empresas-base', data, accessToken, tenant);
}

async function recoverySyndicate(datas, accessToken, tenant) {
	let arrayOfData = [];

	for (let data of datas) {
		let newData = {
			codigo: data.codigoSindicato,
			descricao: data.nomeSindicato
		}

		arrayOfData.push(newData);
	}

	if (arrayOfData.length > 0)
		return await requestCreate('sindicatos', arrayOfData, accessToken, tenant);
}

async function recoveryTributaryStock(datas, accessToken, tenant) {
	let arrayOfData = [];

	for (let data of datas) {
		let newData = {
			codigo: data.codigoLotacao,
			descricao: data.descricaoLotacao,
			descricaoLote: data.descricaoLote
		}

		arrayOfData.push(newData);
	}

	if (arrayOfData.length > 0)
		return await requestCreate('lotacoes-tributarias', arrayOfData, accessToken, tenant);
}

async function recoveryWorkSchedule(datas, accessToken, tenant) {
	let arrayOfData = [];

	for (let data of datas) {
		let newData = {
			codigo: data.codigoHorarioTrabalho,
			descricao: data.descricao,
			horarios: data.horarios
		}

		arrayOfData.push(newData);
	}

	if (arrayOfData.length > 0)
		return await requestCreate('horarios-trabalho', arrayOfData, accessToken, tenant);
}

async function recoveryVariableEvent(datas, accessToken, tenant) {
	let arrayOfData = [];

	for (let data of datas) {
		let newData = {
			codigo: data.codigoEvento,
			descricao: data.descricaoEvento,
			tipoFolha: data.codigoTipoFolha
		}

		if (data.unidade == 'V')
			newData.unidade = 1;
		else if (data.unidade == 'H')
			newData.unidade = 2;
		else if (data.unidade == 'Q')
			newData.unidade = 3;

		arrayOfData.push(newData);
	}

	if (arrayOfData.length > 0)
		return await requestCreate('eventos', arrayOfData, accessToken, tenant);
}

async function recoveryFrequencyJustification(datas, accessToken, tenant) {
	let arrayOfData = [];

	for (let data of datas) {
		let newData = {
			codigo: data.codeIntegration,
			descricao: data.description,
			tipo: data.type == 'A' ? 1 : 2
		}

		arrayOfData.push(newData);
	}

	if (arrayOfData.length > 0)
		return await requestCreate('motivos', arrayOfData, accessToken, tenant);
}

async function recoveryEmployee(relations, accessToken, tenant, db, newDb, company) {
	let resultEmployee = {
		inseridos: 0,
		atualizados: 0,
		erros: []
	};

	for (let relation of relations) {
		let employee = await db.collection('Employee').findOne({ _id: relation.employeeId });

		if (employee) {
			logger.info(`Empregado ${employee.name} iniciado.`);

			let contract = await db.collection('Contract').findOne({ employeeId: relation.employeeId, companyRegistry: company.registry });

			if (contract) {
				let data = {
					matricula: contract.enrollment,
					situacao: contract.status,
					inscricao: employee.registry,
					nome: employee.name,
					email: employee.email !== 'carlos.aurelio@tron.com.br' ? 'carlos.aurelio10@tron.com.br' : 'carlos.aurelio@tron.com.br',
					pisPasep: employee.pis,
					contrato: {
						dataAdmissao: moment(contract.admissionDate, 'DD/MM/YYYY').format('YYYYMMDD'),
						dataRescisao: contract.rescissionDate ? (contract.rescissionDate > 18991230 ? contract.rescissionDate : 0) : 0,
						dispensadoPonto: contract.dismissedRecordPoints,
						motivoDispensadoPonto: contract.dismissedRecordPointsReason,
						utilizaPapelata: contract.curlpaperRecordPoint,
						departamentoId: '',
						cargoId: ''
					},
					horario: {
						horarioTrabalhoId: ''
					}
				};

				if (contract.job) {
					let cargoId = await requestGetCargo(contract.job, accessToken, tenant);

					if (cargoId)
						data.contrato.cargoId = cargoId;
					else
						delete data.contrato.cargoId;
				}

				if (contract.department) {
					let departamentoId = await requestGetDepartamento(contract.department, accessToken, tenant);

					if (departamentoId)
						data.contrato.departamentoId = departamentoId;
					else
						delete data.contrato.departamentoId;
				}

				if (contract.workSchedule && contract.workSchedule.scheduleDescription) {
					let horarioId = await requestGetHorario(contract.workSchedule.scheduleDescription, accessToken, tenant);

					if (horarioId)
						data.horario.horarioTrabalhoId = horarioId;
					else
						delete data.horario;
				}

				const result = await requestCreateEmployee(data, accessToken, tenant);

				if (result) {
					let inscricao = undefined;

					if (result.sucesso) {
						resultEmployee.inseridos += 1;
						inscricao = data.inscricao;
					} else {
						inscricao = await requestGetEmployee(employee.registry, accessToken, tenant);
					}

					if (inscricao) {
						let upserted = 0;

						console.log(chalk.blue('\nIniciando a importação das Frequencias...'));
						let frequencies = await db.collection('Frequency').find({ employeeId: relation.employeeId, companyRegistry: company.registry }).toArray();
						if (frequencies.length > 0)
							await recoveryFrequency(frequencies, inscricao, newDb, accessToken, tenant);

						console.log(chalk.blue('\nIniciando a importação das Programações de Férias...'));
						let controlVacations = await db.collection('ControlVacation').find({ employeeId: relation.employeeId, companyId: company._id }).toArray();
						if (controlVacations.length > 0) {
							upserted = await recoveryControlVacation(controlVacations, inscricao, accessToken, tenant);

							console.log(chalk.blue(`Upserted: ${upserted}\nFinalizada a importação das Programações de Férias.`));
						}

						console.log(chalk.blue('\nIniciando a importação dos Afastamentos...'));
						let absences = await db.collection('Absence').find({ employeeId: relation.employeeId, companyId: company._id }).toArray();
						if (absences.length > 0) {
							upserted = await recoveryAbsence(absences, inscricao, accessToken, tenant);

							console.log(chalk.blue(`Upserted: ${upserted}\nFinalizada a importação dos Afastamentos.`));
						}

						console.log(chalk.blue('\nIniciando a importação dos Avisos Prévios...'));
						let previousNotices = await db.collection('PreviousNotice').find({ employeeId: relation.employeeId, companyId: company._id }).toArray();
						if (previousNotices.length > 0) {
							upserted = await recoveryPreviousNotice(previousNotices, inscricao, accessToken, tenant);

							console.log(chalk.blue(`Upserted: ${upserted}\nFinalizada a importação dos Avisos Prévios.`));
						}

						console.log(chalk.blue('\nIniciando a importação das Justificativas de Falta...'));
						let justificationRequests = await db.collection('JustificationRequests').find({ employeeId: relation.employeeId, companyRegistry: company.registry }).toArray();
						if (justificationRequests.length > 0) {
							upserted = await recoveryJustificationRequest(justificationRequests, inscricao, db, accessToken, tenant);

							console.log(chalk.blue(`Upserted: ${upserted}\nFinalizada a importação das Justificativas de Falta.`));
						}
					}
				}
			}

			logger.info(`Empregado ${employee.name} finalizado.`);
		}
	}

	return resultEmployee;
}

async function recoveryFrequency(frequencies, inscricao, newDb, accessToken, tenant) {
	let frequenciesArray = [];

	for (let frequency of frequencies) {
		let newFrequency = {
			_id: mongo.Binary(crypto.randomBytes(8).toString("hex"), 3),
			Inscricao: inscricao,
			Data: frequency.data,
			TipoDia: buscarTipoDia(frequency.tipoDia)
		};

		if (frequency.cargaHoraria)
			newFrequency.CargaHoraria = frequency.cargaHoraria;

		if (frequency.debito)
			newFrequency.Debito = frequency.debito;

		if (frequency.credito)
			newFrequency.Credito = frequency.credito;

		if (frequency.saldo)
			newFrequency.Saldo = frequency.saldo;

		if (frequency.tipoSaldo)
			newFrequency.TipoSaldo = frequency.tipoSaldo;

		if (frequency.toleranciaEntradaSaida)
			newFrequency.ToleranciaEntradaSaida = frequency.toleranciaEntradaSaida;

		if (frequency.toleranciaMaximaDiaria)
			newFrequency.ToleranciaMaximaDiaria = frequency.toleranciaMaximaDiaria;

		if (frequency.horasTrabalhadas)
			newFrequency.HorasTrabalhadas = frequency.horasTrabalhadas;

		if (frequency.observacao)
			newFrequency.Observacao = frequency.observacao;

		if ((frequency.registries) && (frequency.registries.length > 0)) {
			newFrequency.Registros = [];

			frequency.registries.forEach(registro => {
				newFrequency.Registros.push({
					Ordem: registro.sequencia,
					Horario: registro.horario,
					Hora: registro.registro,
					Origem: 1
				});
			});
		}
		frequenciesArray.push(newFrequency);
	}

	await newDb.collection('FrequenciaTemp').insertMany(frequenciesArray);
	await requestCreate('frequencias/recuperar', null, accessToken, tenant);
}

async function recoveryControlVacation(controlVacations, inscricao, accessToken, tenant) {
	let upserted = 0;

	for (let controlVacation of controlVacations) {
		let newControlVacation = {
			inscricao: inscricao,
			dataGozoInicial: controlVacation.dataGozoInicial,
			dataGozoFinal: controlVacation.dataGozoFinal,
			situacao: controlVacation.status
		}

		if ((controlVacation.dataAbonoInicial) && (controlVacation.dataAbonoInicial > 20000101))
			newControlVacation.dataAbonoInicial = controlVacation.dataAbonoInicial;

		if ((controlVacation.dataAbonoFinal) && (controlVacation.dataAbonoFinal > 20000101))
			newControlVacation.dataAbonoFinal = controlVacation.dataAbonoFinal;

		if (controlVacation.descontarFaltaFerias)
			newControlVacation.descontarFaltaFerias = controlVacation.descontarFaltaFerias;

		if (controlVacation.pagarDecimoTerceiro)
			newControlVacation.pagarDecimoTerceiro = controlVacation.pagarDecimoTerceiro;

		if (controlVacation.observacao)
			newControlVacation.observacao = controlVacation.observacao;

		if (controlVacation.motivoStatus)
			newControlVacation.retorno = controlVacation.motivoStatus;

		const result = await requestCreate('programacoes-ferias', newControlVacation, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);

		}
	}

	return upserted;
}

async function recoveryAbsence(absences, inscricao, accessToken, tenant) {
	let upserted = 0;

	for (let absence of absences) {
		let newAbsence = {
			inscricao: inscricao,
			situacao: absence.status
		}

		if (absence.codigoAfastamento)
			newAbsence.codigoAfastamento = getCodigoAfastamento(absence.codigoAfastamento);

		if ((absence.dataRequerimento) && (absence.dataRequerimento > 20000101))
			newAbsence.dataRequerimento = absence.dataRequerimento;

		if ((absence.dataSaida) && (absence.dataSaida > 20000101))
			newAbsence.dataSaida = absence.dataSaida;

		if ((absence.dataRetorno) && (absence.dataRetorno > 20000101))
			newAbsence.dataRetorno = absence.dataRetorno;

		if ((absence.dataBaixa) && (absence.dataBaixa > 20000101))
			newAbsence.dataBaixa = absence.dataBaixa;

		if (absence.observacao)
			newAbsence.observacao = absence.observacao;

		if (absence.motivoStatus)
			newAbsence.retorno = absence.motivoStatus;

		const result = await requestCreate('afastamentos', newAbsence, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);

		}
	}

	return upserted;
}

async function recoveryPreviousNotice(previousNotices, inscricao, accessToken, tenant) {
	let upserted = 0;

	for (let previousNotice of previousNotices) {
		let newPreviousNotice = {
			inscricao: inscricao,
			situacao: previousNotice.status
		}

		if (previousNotice.tipoAviso)
			newPreviousNotice.tipoAviso = previousNotice.tipoAviso;

		if ((previousNotice.dataCiente) && (previousNotice.dataCiente > 20000101))
			newPreviousNotice.dataCiente = previousNotice.dataCiente;

		if ((previousNotice.dataHomologacao) && (previousNotice.dataHomologacao > 20000101))
			newPreviousNotice.dataHomologacao = previousNotice.dataHomologacao;

		if ((previousNotice.dataAviso) && (previousNotice.dataAviso > 20000101))
			newPreviousNotice.dataAviso = previousNotice.dataAviso;

		if ((previousNotice.dataRescisao) && (previousNotice.dataRescisao > 20000101))
			newPreviousNotice.dataRescisao = previousNotice.dataRescisao;

		if (previousNotice.tipoReducao)
			newPreviousNotice.tipoReducao = previousNotice.tipoReducao;

		if (previousNotice.observacao)
			newPreviousNotice.observacao = previousNotice.observacao;

		if (previousNotice.motivoStatus)
			newPreviousNotice.retorno = previousNotice.motivoStatus;

		const result = await requestCreate('avisos-previos', newPreviousNotice, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);
		}
	}

	return upserted;
}

async function recoveryJustificationRequest(justificationRequests, inscricao, db, accessToken, tenant) {
	let upserted = 0;

	for (let justificationRequest of justificationRequests) {
		let newJustificationRequest = {
			id: justificationRequest._id,
			inscricao: inscricao,
			data: justificationRequest.registryDate,
			observacao: justificationRequest.description
		}

		if (justificationRequest.status == 'pendente') {
			newJustificationRequest.situacao = 'Pendente';
		} else if (justificationRequest.status == 'recusada') {
			newJustificationRequest.situacao = 'Recusado';
		} else if (justificationRequest.status == 'aceita') {
			newJustificationRequest.situacao = 'Aceita';
		} else if (justificationRequest.status == 'processada') {
			newJustificationRequest.situacao = 'Processado';
		}

		if (justificationRequest.sequences)
			newJustificationRequest.sequencia = justificationRequest.sequences;

		if (justificationRequest.fileType)
			newJustificationRequest.extensao = `.${justificationRequest.fileType}`;

		if (justificationRequest.frequencyJustificationId) {
			let frequencyJustification = await db.collection('FrequencyJustification').findOne({ _id: justificationRequest.frequencyJustificationId });

			if (frequencyJustification) {
				let motivoId = await requestGetMotivo(frequencyJustification.codeIntegration, accessToken, tenant);

				if (motivoId)
					newJustificationRequest.motivoId = motivoId;
			}
		}

		const result = await requestCreate('justificativas-frequencia/recuperar', newJustificationRequest, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);
		}
	}

	return upserted;
}

async function recoveryFinancialSheet(financialSheets, accessToken, tenant) {
	let upserted = 0;

	for (let financialSheet of financialSheets) {
		let newFinancialSheet = {
			id: financialSheet._id,
			tipo: 1,
			periodo: 1,
			ano: financialSheet.year
		};

		if (financialSheet.type.trim() == 'Empregadores')
			newFinancialSheet.tipo = 2;
		else if (financialSheet.type.trim() == 'Autônomos')
			newFinancialSheet.tipo = 3;
		else if (financialSheet.type.trim() == 'Empresa Selecionada')
			newFinancialSheet.tipo = 4;

		if (financialSheet.period.trim() == 'Segundo Semestre')
			newFinancialSheet.periodo = 2;
		else if (financialSheet.period.trim() == 'Anual')
			newFinancialSheet.periodo = 3;

		const result = await requestCreate('fichas-financeira/recuperar', newFinancialSheet, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);
		}
	}

	return upserted;
}

async function recoveryFinancialLiquidSheet(financialLiquidSheets, accessToken, tenant) {
	let upserted = 0;

	for (let financialLiquidSheet of financialLiquidSheets) {
		let newFinancialLiquidSheet = {
			id: financialLiquidSheet._id,
			dataInicial: financialLiquidSheet.initialDate,
			dataFinal: financialLiquidSheet.finalDate,
			tipo: 2
		};

		if (financialLiquidSheet.type.trim() == 'Adiantamento')
			newFinancialLiquidSheet.tipo = 1;
		else if (financialLiquidSheet.type.trim() == 'Mensal')
			newFinancialLiquidSheet.tipo = 2;
		else if (financialLiquidSheet.type.trim() == 'Férias')
			newFinancialLiquidSheet.tipo = 3;
		else if (financialLiquidSheet.type.trim() == '13º Salário')
			newFinancialLiquidSheet.tipo = 4;
		else if (financialLiquidSheet.type.trim() == 'Afastamento')
			newFinancialLiquidSheet.tipo = 5;
		else if (financialLiquidSheet.type.trim() == 'Rescisão')
			newFinancialLiquidSheet.tipo = 6;
		else if (financialLiquidSheet.type.trim() == 'PPR')
			newFinancialLiquidSheet.tipo = 7;
		else if (financialLiquidSheet.type.trim() == 'Todos')
			newFinancialLiquidSheet.tipo = 8;

		const result = await requestCreate('liquidos-folha/recuperar', newFinancialLiquidSheet, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);
		}
	}

	return upserted;
}

async function recoverySheetMap(sheetMaps, accessToken, tenant) {
	let upserted = 0;

	for (let sheetMap of sheetMaps) {
		let newSheetMap = {
			id: sheetMap._id,
			dataInicial: sheetMap.initialDate,
			dataFinal: sheetMap.finalDate,
			tipos: sheetMap.type.split(',')
		};

		const result = await requestCreate('mapas-folha/recuperar', newSheetMap, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);
		}
	}

	return upserted;
}

async function recoverySheetSummary(sheetSummaries, accessToken, tenant) {
	let upserted = 0;

	for (let sheetSummary of sheetSummaries) {
		let newSheetSummary = {
			id: sheetSummary._id,
			dataInicial: sheetSummary.initialDate,
			dataFinal: sheetSummary.finalDate,
			tipos: sheetSummary.type.split(',')
		};

		const result = await requestCreate('resumos-folha/recuperar', newSheetSummary, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);
		}
	}

	return upserted;
}

async function recoveryEventReport(eventReports, accessToken, tenant) {
	let upserted = 0;

	for (let eventReport of eventReports) {
		let newEventReport = {
			id: eventReport._id,
			data: eventReport.reportDate
		};

		const result = await requestCreate('eventos-ponto/recuperar', newEventReport, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);
		}
	}

	return upserted;
}

async function recoveryIndividualTaxpayer(individualsTaxpayers, accessToken, tenant) {
	let upserted = 0;

	for (let individualTaxpayer of individualsTaxpayers) {
		let newIndividualTaxpayer = {
			nome: individualTaxpayer.name,
			inscricao: individualTaxpayer.inscription,
			situacao: individualTaxpayer.status,
			dataInicioAtividade: individualTaxpayer.startDate,
			tipo: individualTaxpayer.typeCode + 1,
			categoria: individualTaxpayer.categoryCode,
			email: 'carlos.aurelio2@tron.com.br'
		}

		if (individualTaxpayer.socialSecurityDescription)
			newIndividualTaxpayer.regimePrevidenciario = individualTaxpayer.socialSecurityDescription;

		if ((individualTaxpayer.socialSecurityStart) && (individualTaxpayer.socialSecurityStart > 20000101))
			newIndividualTaxpayer.dataInicioRegimePrevidenciario = individualTaxpayer.socialSecurityStart;

		if ((individualTaxpayer.payment) && isNumber(individualTaxpayer.payment))
			newIndividualTaxpayer.remuneracao = individualTaxpayer.payment;

		if (individualTaxpayer.inssInscription)
			newIndividualTaxpayer.pisPasep = individualTaxpayer.inssInscription;

		const result = await requestCreate('contribuintes-individuais', newIndividualTaxpayer, accessToken, tenant);

		if (result) {
			if (result.sucesso)
				upserted += 1;
			else logger.error(result);

		}
	}

	return upserted;
}

async function recoveryManager(managers, accessToken, tenant, db) {
	let upserted = 0;

	for (let manager of managers) {
		let managerEmployee = await db.collection('Employee').findOne({ _id: manager.employeeId });

		if (managerEmployee) {
			let pessoaGestorId = await requestGetPessoa(managerEmployee.registry, accessToken, tenant);

			let newManager = {
				pessoaGestorId: pessoaGestorId,
				descricao: manager.department,
				equipe: []
			}

			let relations = await db.collection('ManagerEmployees').find({ managerId: manager._id }).toArray();

			for (let relation of relations) {
				let employee = await db.collection('Employee').findOne({ _id: relation.employeeId });

				if (employee)
					newManager.equipe.push(employee.registry);
			}

			if (newManager.equipe.length > 0) {
				const result = await requestCreate('gestores', newManager, accessToken, tenant);

				if (result) {
					if (result.sucesso)
						upserted += 1;
					else logger.error(result);

				}
			}
		}
	}

	return upserted;
}

async function importData(accessToken, tenant) {
	let client = await mongo.MongoClient.connect('mongodb://localhost:27017/');
	assert.notEqual(null, client);

	let db = client.db('db_connect_api');
	assert.notEqual(null, db);
	assert.ok(db.serverConfig.isConnected());

	let newDb = client.db('DB_06006848000104');
	assert.notEqual(null, newDb);
	assert.ok(newDb.serverConfig.isConnected());

	try {
		console.log();
		console.log(chalk.yellow('Iniciando a importação dos Dados...'));

		let company = await db.collection('Company').findOne({ registry: '06006848000104' });

		if (company) {
			console.log(chalk.green('\nIniciando a importação da Empresa...'));
			await recoveryCompany(company, accessToken, tenant);

			console.log(chalk.green('\nIniciando a importação dos Sindicatos...'));
			let syndicates = await db.collection('Syndicate').find({ codigoCliente: '00-01126' }).toArray();
			if (syndicates.length > 0) {
				const result = await recoverySyndicate(syndicates, accessToken, tenant);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação dos Sindicatos.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Horarios de Trabalho...'));
			let workSchedules = await db.collection('WorkSchedule').find({ codigoCliente: '00-01126' }).toArray();
			if (workSchedules.length > 0) {
				const result = await recoveryWorkSchedule(workSchedules, accessToken, tenant);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação dos Horarios de Trabalho.`));
			}

			console.log(chalk.green('\nIniciando a importação das Lotações Tributárias...'));
			let tributariesStock = await db.collection('TributaryStockESocial').find({ inscricaoEmpresa: company.registry }).toArray();
			if (tributariesStock.length > 0) {
				const result = await recoveryTributaryStock(tributariesStock, accessToken, tenant);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação das Lotações Tributárias.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Eventos...'));
			let variableEvents = await db.collection('VariableEvent').find({ companyId: company._id }).toArray();
			if (variableEvents.length > 0) {
				const result = await recoveryVariableEvent(variableEvents, accessToken, tenant);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação dos Eventos.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Motivos...'));
			let frequencyJustifications = await db.collection('FrequencyJustification').find({ companyId: company._id }).toArray();
			if (frequencyJustifications.length > 0) {
				const result = await recoveryFrequencyJustification(frequencyJustifications, accessToken, tenant);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação dos Motivos.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Cargos...'));
			let positions = await db.collection('Position').find({ companyRegistry: company.registry }).toArray();
			if (positions.length > 0) {
				const result = await recoveryCad('cargos', positions, accessToken, tenant);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação dos Cargos.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Departamentos...'));
			let departments = await db.collection('Department').find({ companyRegistry: company.registry }).toArray();
			if (departments.length > 0) {
				const result = await recoveryCad('departamentos', departments, accessToken, tenant);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação dos Departamentos.`));
			}

			console.log(chalk.green('\nIniciando a importação das Seções...'));
			let sections = await db.collection('Section').find({ companyRegistry: company.registry }).toArray();
			if (sections.length > 0) {
				const result = await recoveryCad('secoes', sections, accessToken, tenant);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação das Seções.`));
			}

			console.log(chalk.green('\nIniciando a importação das Fichas Financeiras...'));
			let financialSheets = await db.collection('FinancialSheet').find({ companyId: company._id }).toArray();
			if (financialSheets.length > 0) {
				const upserted = await recoveryFinancialSheet(financialSheets, accessToken, tenant);

				if (upserted)
					console.log(chalk.green(`Inseridos: ${upserted}\nFinalizada a importação das Fichas Financeiras.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Liquido da Folha...'));
			let financialLiquidSheets = await db.collection('FinancialLiquidSheet').find({ companyId: company._id }).toArray();
			if (financialLiquidSheets.length > 0) {
				const upserted = await recoveryFinancialLiquidSheet(financialLiquidSheets, accessToken, tenant);

				if (upserted)
					console.log(chalk.green(`Inseridos: ${upserted}\nFinalizada a importação dos Liquido da Folha.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Mapa da Folha...'));
			let sheetMaps = await db.collection('SheetMap').find({ companyId: company._id }).toArray();
			if (sheetMaps.length > 0) {
				const upserted = await recoverySheetMap(sheetMaps, accessToken, tenant);

				if (upserted)
					console.log(chalk.green(`Inseridos: ${upserted}\nFinalizada a importação dos Mapa da Folha.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Resumo da Folha...'));
			let sheetSummaries = await db.collection('SheetSummary').find({ companyId: company._id }).toArray();
			if (sheetSummaries.length > 0) {
				const upserted = await recoverySheetSummary(sheetSummaries, accessToken, tenant);

				if (upserted)
					console.log(chalk.green(`Inseridos: ${upserted}\nFinalizada a importação dos Resumo da Folha.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Eventos do Ponto...'));
			let eventReports = await db.collection('EventReport').find({ companyId: company._id }).toArray();
			if (eventReports.length > 0) {
				const upserted = await recoveryEventReport(eventReports, accessToken, tenant);

				if (upserted)
					console.log(chalk.green(`Inseridos: ${upserted}\nFinalizada a importação dos Eventos do Ponto.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Contribuintes Individuais...'));
			let individualsTaxpayers = await db.collection('IndividualTaxpayer').find({ companyRegistry: company.registry }).toArray();
			if (individualsTaxpayers.length > 0) {
				const upserted = await recoveryIndividualTaxpayer(individualsTaxpayers, accessToken, tenant);

				if (upserted)
					console.log(chalk.blue(`Upserted: ${upserted}\nFinalizada a importação dos Contribuintes Individuais.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Empregados e filhos...'));
			let relations = await db.collection('EmployeeCompany').find({ companyId: company._id }).toArray();
			if (relations.length > 0) {
				const result = await recoveryEmployee(relations, accessToken, tenant, db, newDb, company);

				if (result)
					console.log(chalk.green(`Inseridos: ${result.inseridos}\nAtualizados: ${result.atualizados}\nErros: ${result.erros.length}\nFinalizada a importação das Seções.`));
			}

			console.log(chalk.green('\nIniciando a importação dos Gestores...'));
			let managers = await db.collection('Manager').find({ companyId: company._id }).toArray();
			if (managers.length > 0) {
				const upserted = await recoveryManager(managers, accessToken, tenant, db);

				if (upserted)
					console.log(chalk.blue(`Upserted: ${upserted}\nFinalizada a importação dos Gestores.`));
			}
		}

		console.log();
		console.log(chalk.yellow('Script successfuly executed! Finishing...'));
	} finally {
		if (client) {
			client.close();
		}
	}
};

async function start() {
	const data = await getAuthorization();

	if (data) {
		console.log(chalk.yellow('*****************************************************'));
		await importData(data.accessToken, data.tenant);
		console.log(chalk.yellow('*****************************************************'));
	}
}

start();

// Remover Todos os Relatórios
'use strict';

let axios = require('axios');
let winston = require('winston');

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

function requestDelete(endPoint, id, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			axios({
				method: 'delete',
				url: `http://localhost:6004/api/${endPoint}/${id}`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'Content-Type': 'application/json',
					'ServiceIdentify': tenant
				}
			}).then(response => {
				resolve(response.data.sucesso);
			}).catch(error => {
				resolve(error.response.data);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

function requestGet(endPoint, accessToken, tenant) {
	return new Promise(resolve => {
		try {
			axios({
				method: 'get',
				url: `http://localhost:6004/api/${endPoint}`,
				headers: {
					'Authorization': `Bearer ${accessToken}`,
					'ServiceIdentify': tenant
				}
			}).then(response => {
				resolve(response.data);
			}).catch(error => {
				resolve(false);
			});
		} catch (e) {
			resolve(false);
		}
	});
}

async function deleteData(accessToken, tenant) {
	const endPoints = ['resumos-folha', 'fichas-financeira', 'liquidos-folha', 'mapas-folha', 'resumos-folha'];

	for (let endPoint of endPoints) {
		logger.info(`Removendo ${endPoint}`);
		let relatorios = await requestGet(endPoint, accessToken, tenant)
		if (relatorios.length > 0) {
			for (let relatorio of relatorios) {
				await requestDelete(endPoint, relatorio.id, accessToken, tenant);
			}
		}
	}

	logger.info('Script successfuly executed! Finishing...');
};

async function start() {
	const data = await getAuthorization();

	if (data) {
		logger.info('*****************************************************');
		await deleteData(data.accessToken, data.tenant);
		logger.info('*****************************************************');
	}
}

start();

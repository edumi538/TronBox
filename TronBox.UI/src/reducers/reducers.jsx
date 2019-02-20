import { combineReducers } from 'redux'
import { reducer as reduxFormReducer } from 'redux-form'
import { reducer as toastrReducer } from 'react-redux-toastr'

import AfastamentoReducer from './afastamentoReducer'
import AvisoFeriasReducer from './avisoFeriasReducer'
import AvisoPrevioReducer from './avisoPrevioReducer'
import ContribuinteIndividualReducer from './contribuinteIndividualReducer'
import EmpregadoReducer from './empregadoReducer'
import EnumeradorReducer from './enumeradorReducer'
import EventoPontoRecucer from './eventoPontoReducer'
import FichaFinanceiraReducer from './fichaFinanceiraReducer'
import FolhaPontoReducer from './folhaPontoReducer'
import FrequenciaReducer from './frequenciaReducer'
import GestorReducer from './gestorReducer'
import GestaoFrequenciaReducer from './gestaoFrequenciaReducer'
import IndicadoresReducer from './indicadoresReducer'
import InformeRendimentosReducer from './informeRendimentosReducer'
import JustificativaFaltaReducer from './justificativaFaltaReducer'
import LiquidoFolhaReducer from './liquidoFolhaReducer'
import MapaFolhaReducer from './mapaFolhaReducer'
import MotivoReducer from './motivoReducer'
import PDFViewerReducer from './pdfViewerReducer'
import PontualidadeReducer from './pontualidadeReducer'
import ProgramacaoEventosReducer from './programacaoEventosReducer'
import ProgramacaoFeriasReducer from './programacaoFeriasReducer'
import ReciboReducer from './reciboReducer'
import ResumoFolhaReducer from './resumoFolhaReducer'
import RotaReducer from './rotasReducer'
import UsuarioReducer from './usuarioReducer'

const rootReducer = combineReducers({
    afastamento: AfastamentoReducer,
    avisoFerias: AvisoFeriasReducer,
    avisoPrevio: AvisoPrevioReducer,
    contribuinteIndividual: ContribuinteIndividualReducer,
    empregado: EmpregadoReducer,
    enumerador: EnumeradorReducer,
    eventoPonto: EventoPontoRecucer,
    fichaFinanceira: FichaFinanceiraReducer,
    folhaPonto: FolhaPontoReducer,
    form: reduxFormReducer,
    frequencia: FrequenciaReducer,
    gestor: GestorReducer,
    gestaoFrequencia: GestaoFrequenciaReducer,
    indicadores: IndicadoresReducer,
    informeRendimentos: InformeRendimentosReducer,
    justificativaFalta: JustificativaFaltaReducer,
    liquidoFolha: LiquidoFolhaReducer,
    mapaFolha: MapaFolhaReducer,
    motivo: MotivoReducer,
    pdf: PDFViewerReducer,
    pontualidade: PontualidadeReducer,
    programacaoEventos: ProgramacaoEventosReducer,
    programacaoFerias: ProgramacaoFeriasReducer,
    recibo: ReciboReducer,
    resumoFolha: ResumoFolhaReducer,
    rota : RotaReducer,
    toastr: toastrReducer,
    usuario: UsuarioReducer
})

export default rootReducer

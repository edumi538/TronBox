import { combineReducers } from 'redux'
import { reducer as reduxFormReducer } from 'redux-form'
import { reducer as toastrReducer } from 'react-redux-toastr'

import EmpregadoReducer from './empregadoReducer'
import DefaultReducer from './defaultReducer'
import ProgramacaoFeriasReducer from './programacaoFeriasReducer'
import UsuarioReducer from './usuarioReducer'
import RotaReducer from './rotasReducer'

const rootReducer = combineReducers({
    form: reduxFormReducer,
    toastr: toastrReducer,
    default: DefaultReducer,
    empregado: EmpregadoReducer,
    programacaoFerias: ProgramacaoFeriasReducer,
    usuario: UsuarioReducer,
    rota : RotaReducer
})

export default rootReducer

const INITIAL_STATE = {
    genericReducer: ''
}

export default function(state = INITIAL_STATE, action) {
    switch (action.type) {
        case 'GENERIC_GET':
            return state;
        case 'GENERIC_POST':
            return state;
        case 'GENERIC_PUT':
            return state;
        default:
            return state;
    }
}
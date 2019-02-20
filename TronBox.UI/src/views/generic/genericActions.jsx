import axios from 'axios';
import { hashHistory } from 'react-router';
import { toastr } from 'react-redux-toastr';

import { reset as resetForm, initialize, change } from 'redux-form';

import Util from '../../helpers/util'
import { validaCnpj } from '../../util/validations';
import fetchJsonp from 'fetch-jsonp';
import moment from 'moment';

export const getGeneric = () => {
    return {
        type: 'GENERIC_GET'
    };
};

export const postGeneric = () => {
    return {
        type: 'GENERIC_POST'
    };
};


export const putGeneric = () => {
    return {
        type: 'GENERIC_PUT'
    };
};

import React from 'react';

import { GridCell } from '@progress/kendo-react-grid';
import { Tooltip } from 'antd';

export default function cellWithEditing(edit, remove) {
    return class extends GridCell {
        render() {
            let botaoEditar = '';

            if (edit) {
                botaoEditar = (
                    <Tooltip title="Editar">
                        <button className="btn btn-info btn-xs" style={{ marginRight: 10 }} onClick={() => { edit(this.props.dataItem); }} >
                            <i className="fa fa-edit fa-fw"></i>
                        </button>
                    </Tooltip>
                );
            }

            let botaoRemover = '';

            if (remove) {
                botaoRemover = (
                    <Tooltip title="Excluir">
                        <button className="btn btn-danger btn-xs" style={{ marginRight: 10 }} onClick={() => { remove(this.props.dataItem); }} >
                            <i className="fa fa-times fa-fw"></i>
                        </button>
                    </Tooltip>
                );
            }

            return (
                <td style={{ textAlign: 'center' }} >
                    { botaoEditar }
                    { botaoRemover }
                </td>
            );
        }
    };
}

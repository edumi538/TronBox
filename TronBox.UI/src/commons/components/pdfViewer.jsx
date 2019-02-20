import React, { Component } from 'react';
import { connect } from 'react-redux';

import PDFViewerReact from 'mgr-pdf-viewer-react'

import { setEstadoInicial, setEscala } from '../../actions/pdfViewerActions'
import CustomNavigation, { CustomPrevButton, CustomNextButton, CustomPages } from './pdfViewerNavigation'

export class PDFViewer extends Component {

    constructor(props) {
        super(props);
    }

    componentWillUnmount() {
        this.props.setEstadoInicial()
    }

    diminuirEscala() {
        if (this.props.pdf.escala > 0) {
            this.props.setEscala(this.props.pdf.escala - 0.25)
        }
    }

    aumentarEscala() {
        if (this.props.pdf.escala < 3) {
            this.props.setEscala(this.props.pdf.escala + 0.25)
        }
    }

    fit() {
        this.props.setEscala(2.0)
    }

    render() {
        if (this.props.pdf.pdfUrl) {
            return (
                <div>
                    <div className="text-center pdf-toolbar">
                        <div className="btn-group">
                            <button className="btn btn-white" onClick={() => this.diminuirEscala()}><i className="fa fa-search-minus"></i> <span className="hidden-xs">Diminuir</span></button>
                            <button className="btn btn-white" onClick={() => this.aumentarEscala()}><i className="fa fa-search-plus"></i> <span className="hidden-xs">Aumentar</span> </button>
                            <button className="btn btn-white" onClick={() => this.fit()}> 100%</button>
                        </div>
                    </div>

                    <div className="m-t-md">
                        <PDFViewerReact
                            document={{
                                url: this.props.pdf.pdfUrl
                            }}
                            scale={this.props.pdf.escala}
                            navigation={CustomNavigation}
                        />
                    </div>
                </div>
            )
        } else {
            return <div></div>
        }
    }
}

const mapStateToProps = (state) => ({ pdf: state.pdf });

const mapDispatchToProps = {
    setEstadoInicial,
    setEscala
};

export default connect(mapStateToProps, mapDispatchToProps)(PDFViewer);
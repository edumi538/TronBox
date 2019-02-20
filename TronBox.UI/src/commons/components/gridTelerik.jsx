import React, { Component } from 'react';

import { Checkbox } from 'antd'

import { Button } from '@progress/kendo-react-buttons';

import { Grid, GridColumn, GridToolbar } from '@progress/kendo-react-grid';

import { ExcelExport, ExcelExportColumn } from '@progress/kendo-react-excel-export';
import { GridPDFExport, PDFExport, savePDF } from '@progress/kendo-react-pdf';

import { IntlProvider, load, LocalizationProvider, loadMessages } from '@progress/kendo-react-intl';

import likelySubtags from 'cldr-core/supplemental/likelySubtags.json';
import currencyData from 'cldr-core/supplemental/currencyData.json';
import weekData from 'cldr-core/supplemental/weekData.json';

import numbers from 'cldr-numbers-full/main/pt/numbers.json';
import currencies from 'cldr-numbers-full/main/pt/currencies.json';
import caGregorian from 'cldr-dates-full/main/pt/ca-gregorian.json';
import dateFields from 'cldr-dates-full/main/pt/dateFields.json';
import timeZoneNames from 'cldr-dates-full/main/pt/timeZoneNames.json';

import { toastr } from 'react-redux-toastr'

load(
    likelySubtags,
    currencyData,
    weekData,
    numbers,
    currencies,
    caGregorian,
    dateFields,
    timeZoneNames
);

import ptMessages from './pt.json';
loadMessages(ptMessages, 'pt');

import { process } from '@progress/kendo-data-query';

export default class App extends Component {
    _export;
    _grid;
    gridPDFExport;
    pdfExportComponent;

    constructor(props) {
        super(props);

        const dataState = { skip: 0, take: 10 };


        this.state = {
            dataResult: process(this.props.dataSource, dataState),
            dataState: dataState,
            pdfExportRequested: false,
            repeatHeader: false
        };

        this.exportToExcel = () => {
            this._export.save(this.state.dataResult);
        }

        this.handleRepeatHeader = () => {
            this.setState({
                repeatHeader: !this.state.repeatHeader
            })
        }

        this.exportPDF = () => {

            raisePDFExportRequestedFlag();

            let exportToPdf = false;

            if (!this._grid.props.sort && !this._grid.props.group) {
                exportToPdf = true
            } else {
                if (this._grid.props.sort) {
                    if (this._grid.props.sort.length > 0) {
                        toastr.error('Exportar para PDF', 'Não é possível exportar os dados quando estão ordenados!')
                    } else {
                        exportToPdf = true;
                    }
                }

                if (this._grid.props.group) {
                    if (this._grid.props.group.length > 0) {
                        toastr.error('Exportar para PDF', 'Não é possível exportar os dados quando estão agrupados!')
                    } else {
                        exportToPdf = true;
                    }
                }
            }

            if (exportToPdf) {
                setTimeout(
                    () => { this.gridPDFExport.save(this.state.dataResult, lowerPDFExportRequestedFlag); },
                    250
                );
            }

            lowerPDFExportRequestedFlag();
        }

        const raisePDFExportRequestedFlag = () => {
            this.setState({ pdfExportRequested: true });
        }

        const lowerPDFExportRequestedFlag = () => {
            this.setState({ pdfExportRequested: false });
        }
    }

    render() {

        const dataStateChange = (event) => {

            this.setState({
                dataResult: process(this.props.dataSource, event.data),
                dataState: event.data
            });
        }

        const { filterable, groupable, pageSizes, resizable, reorderable, fileNameExport, height, exportToPdf } = this.props

        const gridKendo = (
            <Grid
                style={{ height: `${height}px` }}
                group={this.state.dataState.group}
                total={this.state.dataResult.total}
                groupable={groupable}
                resizable={resizable}
                reorderable={reorderable}
                sortable
                sort={this.state.dataState.sort}
                ref={(grid) => this._grid = grid}
                pageable={{ pageSizes }}
                onDataStateChange={dataStateChange}
                filterable={filterable}
                {...this.state.dataState}
                data={this.state.dataResult}
            >
                {exportToPdf &&
                    < GridToolbar >
                        {
                            exportToPdf &&
                            <span>
                                <Button
                                    iconClass="fa fa-file-excel-o"
                                    //look="outline"
                                    onClick={() => this.exportToExcel()}
                                    title="Exportar para Excel"
                                > Excel </Button>
                            </span>
                        }

                        {exportToPdf &&
                            <span
                                style={{
                                    paddingLeft: 5
                                }}
                            >
                                <Button
                                    iconClass="fa fa-file-pdf-o"
                                    //look="outline"
                                    onClick={this.exportPDF}
                                    title="Exportar para PDF"
                                    disabled={this.state.pdfExportRequested}
                                > PDF </Button>
                                <span
                                    style={{
                                        paddingLeft: 5,
                                    }}
                                >
                                    <Checkbox value={this.state.repeatHeader} onChange={this.handleRepeatHeader} />
                                    <label style={{ marginLeft: 5 }}>Repetir cabeçalho</label>
                                </span>
                            </span>
                        }
                    </GridToolbar>
                }

                {this.props.children}
            </Grid >

        )

        return (
            <div>
                <ExcelExport
                    data={this.state.dataResult}
                    ref={(exporter) => { this._export = exporter }}
                    fileName={`${fileNameExport}.xlsx`}
                    filterable={filterable}
                >
                    {
                        this.props.children.map(column => {
                            return (
                                <ExcelExportColumn
                                    key={column.key}
                                    field={column.props.field}
                                    title={column.props.title}
                                    width={column.width}
                                />
                            )
                        })
                    }
                    <LocalizationProvider language="pt">
                        <IntlProvider locale="pt" >
                            {gridKendo}
                        </IntlProvider>
                    </LocalizationProvider>
                </ExcelExport>

                <LocalizationProvider language="pt">
                    <IntlProvider locale="pt" >

                        <GridPDFExport
                            pageTemplate={PageTemplate}
                            fileName={`${fileNameExport}.pdf`}
                            paperSize="A4"
                            scale={0.5}
                            margin="5mm"
                            title={fileNameExport}
                            author="Tron Informártica"
                            creator="Tron Informártica"

                            repeatHeaders={this.state.repeatHeader}
                            ref={(element) => { this.gridPDFExport = element; }}

                        >
                            {
                                this.props.children.map(column => {

                                    if (column.props.exportToPdf) {
                                        return (
                                            <GridColumn
                                                key={column.key}
                                                field={column.props.field}
                                                title={column.props.title}
                                                width={column.props.width}
                                                filterable={column.props.filterable}
                                                filter={column.props.filter}

                                            />
                                        )
                                    }
                                })
                            }

                            {gridKendo}

                        </GridPDFExport>
                    </IntlProvider>
                </LocalizationProvider>
            </div>
        )

    }
}

class PageTemplate extends React.Component {
    render() {
        return (
            <div
                style={{
                    position: 'static',
                    top: "10px",
                    left: "10px",
                }}>
                Pag {this.props.pageNum} de {this.props.totalPages}
            </div>
        )
    }
}

import React from 'react'
import PropTypes from 'prop-types'

export const CustomPrevButton = (props) => {
    const {
        page,
        pages,
        handlePrevClick
    } = props

    let disabled = false

    if (pages === 1)
        return <div></div>
    else if (page === 1)
        disabled = true

    return <button className="btn btn-white" onClick={handlePrevClick} disabled={disabled}><i className="fa fa-long-arrow-left"></i> <span className="hidden-xs">Anterior</span></button>
}
CustomPrevButton.propTypes = {
    page: PropTypes.number.isRequired,
    pages: PropTypes.number.isRequired,
    handlePrevClick: PropTypes.func.isRequired
}

export const CustomNextButton = (props) => {
    const {
        page,
        pages,
        handleNextClick
    } = props

    let disabled = false

    if (pages === 1)
        return <div></div>
    else if (page === pages)
        disabled = true

    return <button className="btn btn-white" onClick={handleNextClick} disabled={disabled}><i className="fa fa-long-arrow-right"></i> <span className="hidden-xs">Próximo</span></button>
}
CustomNextButton.propTypes = {
    page: PropTypes.number.isRequired,
    pages: PropTypes.number.isRequired,
    handleNextClick: PropTypes.func.isRequired
}

const CustomNavigation = (props) => {
    const {
        page,
        pages
    } = props

    const {
        handlePrevClick,
        handleNextClick
    } = props

    return (
        <div className="text-center pdf-toolbar" style={{ marginTop: 5, marginBottom: 5 }}>
            <div className="btn-group">
                <CustomPrevButton page={page} pages={pages} handlePrevClick={handlePrevClick} />
                <CustomNextButton page={page} pages={pages} handleNextClick={handleNextClick} />
            </div>
        </div>
    )
}
CustomNavigation.propTypes = {
    page: PropTypes.number.isRequired,
    pages: PropTypes.number.isRequired,
    handlePrevClick: PropTypes.func.isRequired,
    handleNextClick: PropTypes.func.isRequired
}

export default CustomNavigation
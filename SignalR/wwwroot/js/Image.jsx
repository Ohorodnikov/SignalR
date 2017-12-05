class EditDescription extends React.Component {
    constructor(props) {
        super(props);
        this.click = this.click.bind(this);
        this.hide = this.props.changeHandler;
        console.log(this.props.data.description);        
    }
    hide() {

    }
    click() {
        console.log(this.props.data.id);
        //this.hide(this.refs.descInput.value);
        var model = {
            id: this.props.data.id,
            description: this.refs.descInput.value
        }
        $.ajax({
            url: '/Home/Image/',
            type: 'post',
            dataType: "json",
            data: model,
            success: function (result) {                
                if (result === "success") {
                    
                    debugger;
                    this.hide(this.refs.descInput.value);
                }
            }.bind(this)
        });
    }
    render() {
        return (
            <div className="input">
                <form onSubmit = {this.click}>
                    <input ref="descInput" type="text" placeholder="Description" />
                    <button type="button" onClick={this.click}>Save</button>
                </form>
            </div>
        );
    }
}   

class Description extends React.Component {
    constructor(props) {
        super(props);
        this.click = this.click.bind(this);        
    }
    click() {        
        this.props.changeHandler();
    }
    render() {
        return (
            <div className="Info">
                <div className="Desc">
                    {this.props.data}
                </div>
                <div>
                    <button type="button" onClick={this.click}>Edit</button>
                </div>
            </div>
        );
    }
}

class Image extends React.Component {
    constructor(props) {
        super(props);
        this.state = { show: false, desc: this.props.data.description }
        this.showEdit = this.showEdit.bind(this);
        this.showInfo = this.showInfo.bind(this);
    }
    showInfo(description) {  
        debugger;
        this.setState({ show: false, desc: description });
        console.log("Show info");
    }
    showEdit() {
        console.log("Show edit");
        this.setState({ show: true });
    }
    render() {
        return (            
            <div className="image">
                <div>
                    <img src={"data:image / png;base64, " + this.props.data.base64} />
                </div>
                <div className="Info">
                {
                    !this.state.show && <Description data={this.state.desc} changeHandler={this.showEdit.bind(this)}/>
                }
                </div>
                <div className="edit">
                {   
                    this.state.show && <EditDescription data={this.props.data} changeHandler={this.showInfo.bind(this)}/>                        
                }
                </div>
            </div>            
        );
    }
}

class Grid extends React.Component {
    constructor(props) {
        super(props);
        console.log(props);
    }
    render(){        
        return <Image data={this.props.data}></Image>;
    }
}

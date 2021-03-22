import React from 'react';
import axios from 'axios';
import Cookies from './../../lib/Cookies';
import Layout from './../Layout/Layout';
var inlineBlock = { "display": "inline-block" };
export default class Login extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            email: "",
            password: "",
            loading: false,
            warnText: null
        };
    }

    onSubmit(event) {
        this.setState({ loading: true });
        event.preventDefault();
        axios.post(process.env.REACT_APP_API_HOST + '/api/User/login', { email: this.state.email, password: this.state.password })
            .then(res => {
                console.log(res);
                Cookies.setCookie("JWT_Token", res.data.token, 1);
                window.location.href = "";
            }).catch(err => {
                console.log(err.response.status);
                if (err.response.status === 401)
                    this.setState({ warnText: "Wrong email or password combination!" });
                else
                    this.setState({warnText: "Something went wrong!"});
                
            }).then(() => {
                this.setState({ loading: false });
            })
    }

    onChange(event) {
        this.setState({[event.target.name]: event.target.value});
    }

    render() {
        return (
            <Layout>
                <div className="container has-text-centered">
                    <form onSubmit={this.onSubmit.bind(this)}>
                        <div style={inlineBlock}>
                            <div className="has-text-left has-text-weight-bold is-size-3"><h2>Login:</h2></div><br />
                            <p className={this.state.warnText ? "has-text-danger" : "is-hidden"}>{this.state.warnText}</p>
                            <div style={inlineBlock}>
                                <div className="has-text-left"><label >Email:</label></div>
                                <input className="input" onChange={this.onChange.bind(this)} type="email" name="email" value={this.state.email} required></input>
                            </div><br />
                            <div style={inlineBlock}>
                                <div className="has-text-left"><label >Password:</label></div>
                                <input className="input" onChange={this.onChange.bind(this)} type="password" name="password" value={this.state.password} required></input>
                            </div>
                            <br /><br />
                            <div className="has-text-right"><button className="button is-success" type="submit">Submit</button></div>
                        </div>
                    </form>
                </div>
            </Layout>
        );
    }
}

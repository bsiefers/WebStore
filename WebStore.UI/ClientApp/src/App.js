import React, { Component } from 'react';
import { BrowserRouter, Route, Switch } from 'react-router-dom';

import Home from './components/Home/Home';
import AdminPanel from './components/Admin/AdminPanel';
import Product from './components/Product/Product';
import Checkout from './components/Checkout/Checkout';
import Customer from './components/Customer/CustomerDetailsForm';
import Login from './components/Login/Login';
import SignUp from './components/SignUp/SignUp';

export default class App extends Component {

  render () {
      return (
          <BrowserRouter>
              <Route exact path="/" component={Home} />
              <Switch>
                  <Route exact path="/admin" component={AdminPanel} />
                  <Route path="/admin/product" render={() => <AdminPanel panelPage={0} />} />
                  <Route path="/admin/order" render={() => <AdminPanel panelPage={1} />}  />
                  <Route path="/admin/stock" render={() => <AdminPanel panelPage={2} />} />
              </Switch>
              <Route path="/product" component={Product} />
              <Route path="/customer" component={Customer} />
              <Route path="/payment" component={Checkout} />
              <Route path="/login" component={Login} />
              <Route path="/signup" component={SignUp} />
          </BrowserRouter>
    );
  }
}

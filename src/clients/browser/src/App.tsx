import { Action, Location } from 'history';
import * as React from 'react';
import { withRouter } from 'react-router';
import { Route, RouteComponentProps } from 'react-router-dom';
import './App.css';
import SearchResults from './SearchResults';


interface IAppState {
  input: string;
}

class App extends React.Component<RouteComponentProps, IAppState> {

  constructor(props: RouteComponentProps) {
    super(props);
    // console.log(props);
    this.state = { input: "" };


    this.props.history.listen(this.onRouteChange);
  }

  public render() {
    return (
      <div>
        {this._renderNavigationBar()}      
        {this._renderContent()}
      </div>
    );
  }

  private onRouteChange = (location: Location, action: Action) =>
    this.setState({ input: "" });

  private _renderNavigationBar() {
    return (
      <nav className="navbar navbar-inverse" role="navigation">
        <div className="container search-container">
          <div className="row">
            <form className="col-sm-12">

            {this.props.location.pathname === '/' ? this.DisplayNavBar() : this.GoToMainPage()}
            </form>
          </div>
        </div>
      </nav>
    );
  }
  private GoToMainPage() {
    return (
      <a href="/"> &lt; Back </a>
    );
  }
  private DisplayNavBar() {
    return (
      <input type="text"
        className="form-control"
        autoComplete="off"
        placeholder="Search resources like Model, Data or Code"
        id="searchbox"
        onChange={this.handleChange}
      />
    );
  }

  private _renderContent() {
    if (this.state.input.length === 0) {
      return (
        <section role="main" className="container main-container">
          <Route exact={true} path="/" render={this.renderSearch} />
          {this.props.children}
        </section>
      );
    }
    else {
      return (
        <section role="main" className="container main-container">
          <SearchResults input={this.state.input} />
        </section>
      );
    }
  }

  private renderSearch = (props: RouteComponentProps<any>) => (
    <SearchResults input={this.state.input} {...props} />
  );

  private handleChange = (input: React.ChangeEvent<HTMLInputElement>) =>
    this.setState({ input: input.target.value });
}

export default withRouter(App);

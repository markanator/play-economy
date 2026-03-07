import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { Login } from './Login'
import { Logout } from './Logout'
import { AuthorizationPaths, LoginActions, LogoutActions } from './ApiAuthorizationConstants';

const prefix = AuthorizationPaths.ApiAuthorizationPrefix;

function stripPrefix(path) {
  return path.replace(prefix, '') || '/';
}

export default function ApiAuthorizationRoutes() {
  return (
    <Routes>
      <Route path={stripPrefix(AuthorizationPaths.Login)} element={<Login action={LoginActions.Login} />} />
      <Route path={stripPrefix(AuthorizationPaths.LoginFailed)} element={<Login action={LoginActions.LoginFailed} />} />
      <Route path={stripPrefix(AuthorizationPaths.LoginCallback)} element={<Login action={LoginActions.LoginCallback} />} />
      <Route path={stripPrefix(AuthorizationPaths.Profile)} element={<Login action={LoginActions.Profile} />} />
      <Route path={stripPrefix(AuthorizationPaths.Register)} element={<Login action={LoginActions.Register} />} />
      <Route path={stripPrefix(AuthorizationPaths.LogOut)} element={<Logout action={LogoutActions.Logout} />} />
      <Route path={stripPrefix(AuthorizationPaths.LogOutCallback)} element={<Logout action={LogoutActions.LogoutCallback} />} />
      <Route path={stripPrefix(AuthorizationPaths.LoggedOut)} element={<Logout action={LogoutActions.LoggedOut} />} />
    </Routes>
  );
}

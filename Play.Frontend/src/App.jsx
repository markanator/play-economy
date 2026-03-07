import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Catalog } from './components/Catalog';
import { Inventory } from './components/Inventory';
import { Users } from './components/Users';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { AuthorizationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import { ApplicationPaths } from './components/Constants';

import './App.css'

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path='/' element={<Home />} />
        <Route path={ApplicationPaths.CatalogPath} element={<AuthorizeRoute><Catalog /></AuthorizeRoute>} />
        <Route path={ApplicationPaths.InventoryPath} element={<AuthorizeRoute><Inventory /></AuthorizeRoute>} />
        <Route path={ApplicationPaths.UsersPath} element={<AuthorizeRoute><Users /></AuthorizeRoute>} />
        <Route path={`${AuthorizationPaths.ApiAuthorizationPrefix}/*`} element={<ApiAuthorizationRoutes />} />
      </Routes>
    </Layout>
  );
}

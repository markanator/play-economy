import React, { useState, useEffect } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { AuthorizationPaths, QueryParameterNames } from './ApiAuthorizationConstants';
import authService from './AuthorizeService';

export default function AuthorizeRoute({ children }) {
    const [ready, setReady] = useState(false);
    const [authenticated, setAuthenticated] = useState(false);
    const location = useLocation();

    useEffect(() => {
        const subscription = authService.subscribe(() => populateAuthenticationState());
        populateAuthenticationState();
        return () => authService.unsubscribe(subscription);
    }, []);

    async function populateAuthenticationState() {
        const isAuthenticated = await authService.isAuthenticated();
        setReady(true);
        setAuthenticated(isAuthenticated);
    }

    if (!ready) {
        return <div></div>;
    }

    if (authenticated) {
        return children;
    }

    const returnUrl = `${window.location.origin}${location.pathname}${location.search}${location.hash}`;
    const redirectUrl = `${AuthorizationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(returnUrl)}`;
    return <Navigate to={redirectUrl} replace />;
}

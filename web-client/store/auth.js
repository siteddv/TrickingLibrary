﻿const initState = () => ({
  profile: null
})

export const state = initState

const ROLES = {
  MODERATOR: "Mod",
  ADMIN: "Admin",
}

export const getters = {
  authenticated: state => state.profile != null,
  moderator: (state, getters) => getters.authenticated
    && (getters.admin || state.profile.role === ROLES.MODERATOR),
  admin: (state, getters) => getters.authenticated && state.profile.role === ROLES.ADMIN,
}

export const mutations = {
  saveProfile(state, {profile}) {
    state.profile = profile
  }
}

export const actions = {
  initialize({commit}) {
    return this.$axios.$get('/api/users/me')
      .then(profile => commit('saveProfile', {profile}))
      .catch(() => {
      })
  },
  login() {
    if (process.server) return;
    const returnUrl = encodeURIComponent(location.href)
    window.location = `${this.$config.auth.loginPath}?returnUrl=${returnUrl}`
  },
  logout() {
    if (process.server) return;
    window.location = this.$config.auth.logoutPath
  }
}

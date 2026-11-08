/** The four teams that share the app in week 3. The slug is the value sent in the X-Team header. */
export const teams = [
  { slug: 'salj', name: 'Sälj' },
  { slug: 'ekonomi', name: 'Ekonomi' },
  { slug: 'support', name: 'Support' },
  { slug: 'utveckling', name: 'Utveckling' },
] as const

export const defaultTeam = 'utveckling'

///** @type {import('next').NextConfig} */
//const nextConfig = {};
//export default nextConfig;


/** @type {import('next').NextConfig} */
const nextConfig = {
  /*experimental: {
    serverActions: true
  }, */
  logging: {
      fetches: {
        fullUrl: true,
      },
    },
    images: {
      domains: [
        'cdn.pixabay.com'
      ]
    },
    output: 'standalone'

}

export default nextConfig

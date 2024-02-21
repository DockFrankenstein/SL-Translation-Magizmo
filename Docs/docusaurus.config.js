// @ts-check
// `@type` JSDoc annotations allow editor autocompletion and type checking
// (when paired with `@ts-check`).
// There are various equivalent ways to declare your Docusaurus config.
// See: https://docusaurus.io/docs/api/docusaurus-config

import {themes as prismThemes} from 'prism-react-renderer';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'SL Translation Magizmo Docs',
  favicon: '/img/favicon.ico',

  // Set the production url of your site here
  url: 'https://your-docusaurus-site.example.com',
  // Set the /<baseUrl>/ pathname under which your site is served
  // For GitHub pages deployment, it is often '/<projectName>/'
  baseUrl: '/',

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: 'Dock Frankenstein', // Usually your GitHub org/user name.
  projectName: 'SL Translation Magizmo', // Usually your repo name.

  onBrokenLinks: 'warn',
  onBrokenMarkdownLinks: 'warn',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: './sidebars.js',
          path: "docs/main",
          routeBasePath: '/',
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl:
            'https://github.com/facebook/docusaurus/tree/main/packages/create-docusaurus/templates/shared/',
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      }),
    ],
  ],

  plugins: [
    [
      "@docusaurus/plugin-content-docs",
      {
        id: "manual",
        path: "docs/manual",
        routeBasePath: "manual",
      }
    ],
    [
      "@docusaurus/plugin-content-docs",
      {
        id: "contributing",
        path: "docs/contributing",
        routeBasePath: "contributing"
      }
    ]
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      // Replace with your project's social card
      image: 'img/docusaurus-social-card.jpg',
      navbar: {
        title: 'SL: Translation Magizmo',
        logo: {
          alt: 'SLTM logo',
          src: 'img/logo.svg',
        },
        items: [
          {
            label: 'Welcome',
            type: 'doc',
            docId: 'index',
            docsPluginId: "default",
          },
          {
            label: 'Manual',
            type: 'doc',
            docId: 'index',
            docsPluginId: "manual",
          },
          {
            label: 'Contributing',
            type: 'doc',
            docId: 'index',
            docsPluginId: "contributing",
          },
          {
            href: 'https://github.com/DockFrankenstein/SL-Translation-Magizmo',
            label: 'GitHub',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        copyright: `©Dock Frankenstein ${new Date().getFullYear()}. Made with Docusaurus`,
      },
      prism: {
        theme: prismThemes.github,
        darkTheme: prismThemes.dracula,
      },
    }),
};

export default config;

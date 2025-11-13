import { defineConfig, globalIgnores } from 'eslint/config'
import globals from 'globals'
import js from '@eslint/js'
import pluginVue from 'eslint-plugin-vue'

export default defineConfig([
  {
    name: 'app/files-to-lint',
    files: ['src/**/*.{js,mjs,jsx,vue}'],
  },

  // 忽略产物与静态资源目录，避免 lint 误扫 vendor 文件
  globalIgnores([
    '**/node_modules/**',
    '**/dist/**',
    '**/dist-ssr/**',
    '**/coverage/**',
    '**/public/**',
    '**/src/assets/**',
  ]),

  {
    languageOptions: {
      globals: {
        ...globals.browser,
      },
    },
  },

  // 为测试文件提供 Vitest 全局（describe/test/expect/vi），避免 no-undef
  {
    name: 'app/test-files',
    files: ['**/*.spec.js', '**/*.test.js'],
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
        describe: 'readonly',
        test: 'readonly',
        expect: 'readonly',
        vi: 'readonly',
      },
    },
  },

  js.configs.recommended,
  ...pluginVue.configs['flat/essential'],

  // Vue 规则覆盖：允许单词组件名（如通用 Editor）
  {
    name: 'app/vue-rule-overrides',
    files: ['src/**/*.vue'],
    rules: {
      'vue/multi-word-component-names': 'off',
    },
  },

  // 忽略回调/插槽中未使用的 row 变量
  {
    name: 'app/row-unused-override',
    files: ['src/**/*.{js,mjs,jsx,vue}'],
    rules: {
      'no-unused-vars': ['warn', { argsIgnorePattern: '^row$', varsIgnorePattern: '^row$' }],
    },
  },
])

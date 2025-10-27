# Sidebar Menu Style Guide (Element Plus)

This document describes the visual and technical implementation for the admin sidebar menu.

## Visual Principles
- Modern and clean, aligned with existing project aesthetics.
- Clear spacing and hierarchy for better readability.
- Consistent icon + label layout, with subtle transitions.
- Accessible focus styles and responsive adjustments.

## Implementation Summary
- Technology: Element Plus components; SCSS via `<style scoped lang="scss">`.
- File: `src/layout/AdminLayout.vue` (SCSS block)
- Key selectors (scoped):
  - `:deep(.el-menu)` – container spacing & background
  - `:deep(.el-menu-item)` – item spacing, radius, typography
  - States: `&:hover`, `&.is-active`, `&:focus`/`&:focus-visible`

## Design Tokens & Variables
- Color: uses Element Plus CSS vars
  - `--el-color-primary`, `--el-color-primary-light-8`
- Background: `#1f2d3d` (sidebar)
- Typography: default Element Plus font; menu items emphasize readability

## Layout & Spacing
- Item height: 40px (desktop), 36px (mobile)
- Item padding: `0 14px` (desktop), `0 10px` (mobile)
- Item margin: `4px 8px` (desktop), `2px 6px` (mobile)
- Radius: 8px (desktop), 6px (mobile)
- Icon size: 18px (desktop), 16px (mobile)
- Gap between icon and label: 10px

## Interaction States
- Hover: subtle translucent background `rgba(255,255,255,0.08)`
- Active: `background: var(--el-color-primary-light-8)`; inset outline with `var(--el-color-primary)`
- Focus/Focus-visible: soft inner outline for accessibility

## Responsive Behavior
- Breakpoint: `max-width: 768px`
- Reduced paddings, heights and icon sizes to maintain clarity
- No functional changes to collapse behavior

## Accessibility
- Focus indicators provided for keyboard navigation
- Sufficient color contrast on active states

## Maintenance Tips
- Keep all sidebar related styles within `AdminLayout.vue`
- Prefer Element Plus CSS variables for theme consistency
- Avoid hardcoding colors beyond the sidebar background

## Test Checklist
- Desktop & mobile preview via browser dev tools
- Hover/active/focus behaviors render correctly
- Collapsed/expanded modes preserved
- ESLint passes (`npm run lint`)
- Visual check in Chromium & Edge
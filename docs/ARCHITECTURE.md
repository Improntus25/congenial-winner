# Architecture Documentation

## Overview
ModernNesting is built using C# (.NET 8.0) with a modern, modular architecture.

## Core Components

### 1. File Processing Module
- Input file parsing (DXF, SVG, AI, CDR)
- Geometry validation
- Format conversion

### 2. Nesting Engine
- Genetic algorithm implementation
- Collision detection
- Multi-sheet optimization
- Sheet size management

### 3. User Interface
- WPF-based interface
- Real-time preview
- Configuration management
- Progress reporting

### 4. Export Module
- Result visualization
- Report generation
- Export in multiple formats

## Data Flow
1. File Import → Geometry Validation
2. Optimization Configuration
3. Nesting Process
4. Result Generation
5. Export

## Technology Stack
- C# (.NET 8.0)
- WPF for UI
- SkiaSharp for rendering
- Clipper2 for geometry operations

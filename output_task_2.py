// /root/ui-agent-working/src/App.tsx
function App() {
  return (
    <main className="min-h-screen bg-gray-950 flex items-center justify-center relative overflow-hidden">
      {/* gradient background */}
      <div className="absolute inset-0 bg-gradient-to-br from-gray-950 via-gray-900 to-indigo-950" />
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_top,_rgba(99,102,241,0.15)_0%,_transparent_60%)]" />
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_bottom_right,_rgba(139,92,246,0.1)_0%,_transparent_50%)]" />

      {/* grid overlay */}
      <div
        className="absolute inset-0 opacity-[0.03]"
        style={{
          backgroundImage:
            'linear-gradient(rgba(255,255,255,0.8) 1px, transparent 1px), linear-gradient(90deg, rgba(255,255,255,0.8) 1px, transparent 1px)',
          backgroundSize: '60px 60px',
        }}
      />

      <div className="relative z-10 max-w-5xl mx-auto px-6 py-24 text-center">
        <div className="inline-flex items-center gap-2 px-4 py-1.5 rounded-full border border-indigo-500/30 bg-indigo-500/10 text-indigo-300 text-sm font-medium mb-8 tracking-wide">
          <span className="w-1.5 h-1.5 rounded-full bg-indigo-400 animate-pulse" />
          Plataforma de Orquestração de IA
        </div>

        <h1 className="text-5xl sm:text-7xl lg:text-8xl font-bold text-white leading-[1.05] tracking-tight mb-6">
          WGO —{' '}
          <span className="bg-gradient-to-r from-indigo-400 via-violet-400 to-purple-400 bg-clip-text text-transparent">
            Orquestrador
          </span>
          <br />
          de Agentes de IA
        </h1>

        <p className="text-lg sm:text-xl lg:text-2xl text-gray-400 max-w-2xl mx-auto mb-12 leading-relaxed">
          Coordene múltiplos agentes de inteligência artificial em fluxos
          complexos. Automatize decisões, paralelize tarefas e entregue
          resultados com precisão e escala.
        </p>

        <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
          <button
            type="button"
            className="w-full sm:w-auto px-8 py-4 rounded-xl bg-gradient-to-r from-indigo-500 to-violet-500 hover:from-indigo-400 hover:to-violet-400 text-white font-semibold text-lg shadow-lg shadow-indigo-500/25 hover:shadow-indigo-500/40 transition-all duration-200 hover:-translate-y-0.5 active:translate-y-0"
          >
            Começar Agora
          </button>
          <button
            type="button"
            className="w-full sm:w-auto px-8 py-4 rounded-xl border border-gray-700 hover:border-gray-500 text-gray-300 hover:text-white font-semibold text-lg transition-all duration-200 hover:-translate-y-0.5 active:translate-y-0 bg-white/[0.03] hover:bg-white/[0.06]"
          >
            Ver Documentação
          </button>
        </div>

        <div className="mt-20 grid grid-cols-3 gap-8 border-t border-gray-800/60 pt-12 max-w-2xl mx-auto">
          {[
            { value: '99.9%', label: 'Uptime' },
            { value: '<50ms', label: 'Latência média' },
            { value: '∞', label: 'Agentes em paralelo' },
          ].map(({ value, label }) => (
            <div key={label} className="flex flex-col gap-1">
              <span className="text-2xl sm:text-3xl font-bold text-white tracking-tight">
                {value}
              </span>
              <span className="text-sm text-gray-500">{label}</span>
            </div>
          ))}
        </div>
      </div>
    </main>
  )
}

export default App
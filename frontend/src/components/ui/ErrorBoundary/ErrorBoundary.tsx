import { Component, type ErrorInfo, type ReactNode } from 'react'
import { texts } from '@/locales/ru'
import style from './errorBoundary.module.css'

interface Props {
  children: ReactNode
}

interface State {
  error: Error | null
}

export class ErrorBoundary extends Component<Props, State> {
  state: State = { error: null }

  static getDerivedStateFromError(error: Error): State {
    return { error }
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    console.error('Dashboard crashed', error, info.componentStack)
  }

  render() {
    if (!this.state.error) {
      return this.props.children
    }

    return (
      <div className={style.screen}>
        <div className={style.box}>
          <h1 className={style.title}>{texts.states.crashTitle}</h1>
          <p className={style.hint}>{texts.states.crashHint}</p>
          <button type="button" onClick={() => window.location.reload()} className={style.button}>
            {texts.states.reload}
          </button>
        </div>
      </div>
    )
  }
}

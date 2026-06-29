import { NextRequest, NextResponse } from 'next/server'

const BACKEND = process.env.BACKEND_URL ?? 'http://localhost:8080'

async function proxy(req: NextRequest): Promise<NextResponse> {
  const path = req.nextUrl.pathname   // /api/v1/auth/register
  const search = req.nextUrl.search  // ?foo=bar
  const url = `${BACKEND}${path}${search}`

  const headers = new Headers(req.headers)
  // Remove headers that confuse the upstream server
  headers.delete('host')
  headers.delete('x-forwarded-for')
  headers.delete('x-forwarded-host')
  headers.delete('x-forwarded-proto')

  const hasBody = req.method !== 'GET' && req.method !== 'HEAD'
  const body = hasBody ? await req.arrayBuffer() : undefined

  let upstream: Response
  try {
    upstream = await fetch(url, {
      method: req.method,
      headers,
      body: body ? Buffer.from(body) : undefined,
      redirect: 'manual',
      // @ts-expect-error — Node.js fetch needs this to avoid buffering large responses
      duplex: 'half',
    })
  } catch (err) {
    console.error(`[proxy] ${req.method} ${url} — fetch failed:`, err)
    return NextResponse.json(
      { success: false, message: `Proxy error: backend unreachable at ${url}` },
      { status: 502 }
    )
  }

  const responseHeaders = new Headers(upstream.headers)
  // Strip hop-by-hop headers that must not be forwarded
  responseHeaders.delete('transfer-encoding')
  responseHeaders.delete('connection')
  responseHeaders.delete('keep-alive')

  console.log(`[proxy] ${req.method} ${url} → ${upstream.status}`)

  return new NextResponse(upstream.body, {
    status: upstream.status,
    statusText: upstream.statusText,
    headers: responseHeaders,
  })
}

export const GET     = proxy
export const POST    = proxy
export const PUT     = proxy
export const PATCH   = proxy
export const DELETE  = proxy
export const OPTIONS = proxy

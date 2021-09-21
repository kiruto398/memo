import java.io.IOException;
import java.io.InputStream;
import java.util.NoSuchElementException;

public class Main {

	static StringBuilder out = new StringBuilder();

	public static void main(String[] args) {
		FastScanner sc = new FastScanner(System.in);

		int N = sc.nextInt();
		long startTime = System.currentTimeMillis();

		int mod = (int) (10e9+7);

		int ln = 2 << (N*2 +2);

		//DPのメモリ節約
		int[][][] dp = new int[2][2][ln];
		dp[0][0][0] = 1;

		for(int i = 0; i < N; i++) {
			int ii = i%2;
			int ni = ii;

			for(int j = 0; j < N; j++) {
				int jj = j%2;
				int nj = 1 - jj;
				if(j+1 == N) {
					nj = 0;
					ni = 1-ni;
				}


				for(int bits = 0; bits < ln; bits++) {
					if(dp[ii][jj][bits] == 0)
						continue;

					if((1 & bits) == 1) {
						//なにも置けない((i, j) がふさがれている)場合

						int nbits = bits >> 1;

						dp[ni][nj][nbits] += dp[ii][jj][bits];
						dp[ni][nj][nbits] %= mod;
						dp[ii][jj][bits] = 0;
					}else {

						//1*1のタイル
						int nbits = bits >> 1;
						dp[ni][nj][nbits] += dp[ii][jj][bits];
						dp[ni][nj][nbits] %= mod;

						//2*2のタイル
						if(!isNxNok(2, i, j, bits, N)) {
							dp[ii][jj][bits] = 0;
							continue;
						}
						nbits = putNxN(2, i, j, bits, N);
						dp[ni][nj][nbits] += dp[ii][jj][bits];
						dp[ni][nj][nbits] %= mod;

						//3*3のタイル
						if(!isNxNok(3, i, j, bits, N)) {
							dp[ii][jj][bits] = 0;
							continue;
						}
						nbits = putNxN(3, i, j, bits, N);
						dp[ni][nj][nbits] += dp[ii][jj][bits];
						dp[ni][nj][nbits] %= mod;
						dp[ii][jj][bits] = 0;
					}
				}
			}
		}

		//答えはbits = 0に集約されている
		int ans = dp[N%2][0][0];


		long endTime = System.currentTimeMillis();

		out.append(ans);
		System.out.println(out);

	    System.out.println("処理時間：" + (endTime - startTime) + " ms");

	}

	private static int putNxN(int n, int i, int j, int bits, int N) {
		for(int k = 1; k < n*n; k++) {
			int d = k/n * (N-n) + k;

			bits |= 1 << d;
		}

		bits = bits >> 1;

		return bits;
	}

	private static boolean isNxNok(int n, int i, int j, int bits, int N) {
		if(N-i < n || N-j < n)
			return false;

		boolean ret = true;
		for(int k = 0; k < n*n; k++) {
			int d = k/n * (N-n) + k;

			ret &= (1 & bits >> d) == 0;
		}

		return ret;
	}
}

class FastScanner {
    private final InputStream in = System.in;
    private final byte[] buffer = new byte[1024];
    private int ptr = 0;
    private int buflen = 0;
    public FastScanner(InputStream in2) {
	}
	private boolean hasNextByte() {
        if (ptr < buflen) {
            return true;
        }else{
            ptr = 0;
            try {
                buflen = in.read(buffer);
            } catch (IOException e) {
                e.printStackTrace();
            }
            if (buflen <= 0) {
                return false;
            }
        }
        return true;
    }
    private int readByte() { if (hasNextByte()) return buffer[ptr++]; else return -1;}
    private static boolean isPrintableChar(int c) { return 33 <= c && c <= 126;}
    public boolean hasNext() { while(hasNextByte() && !isPrintableChar(buffer[ptr])) ptr++; return hasNextByte();}
    public String next() {
        if (!hasNext()) throw new NoSuchElementException();
        StringBuilder sb = new StringBuilder();
        int b = readByte();
        while(isPrintableChar(b)) {
            sb.appendCodePoint(b);
            b = readByte();
        }
        return sb.toString();
    }
    public long nextLong() {
        if (!hasNext()) throw new NoSuchElementException();
        long n = 0;
        boolean minus = false;
        int b = readByte();
        if (b == '-') {
            minus = true;
            b = readByte();
        }
        if (b < '0' || '9' < b) {
            throw new NumberFormatException();
        }
        while(true){
            if ('0' <= b && b <= '9') {
                n *= 10;
                n += b - '0';
            }else if(b == -1 || !isPrintableChar(b)){
                return minus ? -n : n;
            }else{
                throw new NumberFormatException();
            }
            b = readByte();
        }
    }
    public int nextInt() {
        long nl = nextLong();
        if (nl < Integer.MIN_VALUE || nl > Integer.MAX_VALUE) throw new NumberFormatException();
        return (int) nl;
    }
    public double nextDouble() { return Double.parseDouble(next());}
}
